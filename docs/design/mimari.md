# Tasarım Belgesi — e-Fatura Entegratör Senaryosunda Olay Güdümlü Mikroservis Mimarisi

Sürüm 0.1 — 2026-11-09. Bu belge tez yürütme planındaki H1.3 çıktısıdır; mimari bileşenler, mesaj sözleşmesi, SOAP simülatör sözleşmesi, broker dengeleme parametreleri, güvenilirlik desenleri ve ölçüm tanımlarını sabitler.

## 1. Mimari Bileşenler (C4 — Container düzeyi)

```mermaid
C4_Container
  title e-Fatura entegratör senaryosu — bileşen görünümü

  Person(Kullanıcı, "Test istemcisi / k6", "Fatura gönderim isteği başlatır")

  System_Boundary(Uygulama, "e-Fatura entegratör platformu") {
    Container(Gateway, "Gateway", "ASP.NET Core 8", "REST kabul (202); senkron/asenkron yönlendirme (SYNC_MODE)")
    Container(FaturaServisi, "FaturaServisi", "ASP.NET Core 8 + EF Core", "UBL-TR üretimi, validasyon; invoice + outbox (tek transaction)")
    Container(ZarfServisi, "ZarfServisi", "ASP.NET Core 8", "Zarf paketleme; durum makinesi: created → sent → accepted/rejected")
    Container(GibAdapter, "GibAdapter", "ASP.NET Core 8 + WCF + Polly", "SOAP istemci; üstel geri çekilme; SOAP Fault işleme; DLQ")
    Container(GibSimulator, "GibSimulator", "ASP.NET Core 8 + SoapCore", "SOAP uç nokta; hata enjeksiyonu; bellek-içi depo")
  }

  SystemDb(Postgres, "PostgreSQL 16", "invoice, outbox, processed_messages")
  SystemQueue(Broker, "Broker", "RabbitMQ 3.13 veya Kafka 3.7 (KRaft)")
  System_Ext(Prom, "Prometheus + k6", "Metrik toplama ve yük üretimi")

  Rel(Kullanıcı, Gateway, "POST /api/faturalar")
  Rel(Gateway, FaturaServisi, "senkron: HTTP çağrısı", "REST")
  Rel(Gateway, FaturaServisi, "asenkron: kalıcı yazım", "outbox")
  Rel(FaturaServisi, Postgres, "invoice + outbox", "tek transaction")
  Rel(FaturaServisi, Broker, "outbox → efatura.events", "polling publisher")
  Rel(Broker, ZarfServisi, "efatura.events", "consumer")
  Rel(ZarfServisi, Broker, "efatura.zarf", "publisher")
  Rel(Broker, GibAdapter, "efatura.zarf", "consumer")
  Rel(GibAdapter, GibSimulator, "SendInvoice / GetInvoiceStatus", "SOAP 1.1")
  Rel(GibSimulator, Broker, "durum olayı", "publisher")
  Rel(Broker, ZarfServisi, "durum geri akışı", "consumer")
  Rel(ZarfServisi, FaturaServisi, "final state", "kuyruk üzerinden")
  Rel(Prom, Gateway, "metrik", "scrape")
```

### 1.1 Bileşen tablosu

| Kimlik | Bileşen | Sorumluluk | Teknoloji |
|---|---|---|---|
| C1 | Gateway | REST arayüzü; istek kabulü (202); senkron/asenkron mod yönlendirme (`SYNC_MODE`) | ASP.NET Core 8 |
| C2 | FaturaServisi | UBL-TR üretimi (sabit şablon + rastgele alanlar), validasyon, kalıcılık (invoice + outbox, tek transaction) | ASP.NET Core 8, EF Core, PostgreSQL 16 |
| C3 | ZarfServisi | Zarf paketleme (≤ 20 fatura/zarf), durum makinesi: created → sent → accepted/rejected | ASP.NET Core 8, PostgreSQL |
| C4 | GibAdapter | SOAP istemci; üstel geri çekilmeli retry; SOAP Fault işleme; DLQ yönetimi | ASP.NET Core 8, WCF (System.ServiceModel.Http), Polly |
| C5 | GibSimulator | SOAP uç nokta (3 operasyon, WS-I Basic Profile); hata enjeksiyonu; bellek-içi belge deposu | ASP.NET Core 8, SoapCore |
| C6 | Broker | Mesaj taşıma; transport anahtarı `BROKER_TYPE=rabbitmq\|kafka` | RabbitMQ 3.13 veya Kafka 3.7 (KRaft, tek düğüm) |
| C7 | Ölçüm | Metrik toplama ve yük üretimi | Prometheus 2.x, k6 |

## 2. Veri Akışları

### 2.1 Senkron hat (taban çizgisi)

```
Gateway →(REST) FaturaServisi →(HTTP) ZarfServisi →(SOAP) GibAdapter →(SOAP) GibSimulator
```

Her adım zincirsel ve bloklamadır; E2E gecikme tüm zincirin toplamına eşittir.

### 2.2 Asenkron hat (RabbitMQ / Kafka ortak)

```
Gateway →(REST) FaturaServisi [DB + outbox, tek transaction]
        →(broker: efatura.events) ZarfServisi [zarf paketleme]
        →(broker: efatura.zarf) GibAdapter [SOAP çağrısı]
        →(GibSimulator) durum olayı →(broker geri akışı) ZarfServisi/FaturaServisi final state
```

REST kabul, kalıcı yazım anında döner (202); E2E gecikme `accept_ts → final_ts` arasındaki sanyedir.

## 3. Mesaj Sözleşmesi

JSON, UTF-8; broker-özgü zarflama kullanılmaz (adil karşılaştırma).

```json
{
  "messageId": "6f0e9a2c-1c44-4c6f-9d3e-4a1b2c3d4e5f",
  "envelopeId": "a7e5f9c2-8b1d-4e2f-a3b4-c5d6e7f8a9b0",
  "eventType": "invoice-created",
  "vkn": "1234567890",
  "invoiceCount": 3,
  "payloadSha256": "64 haneli hexadecimal",
  "issuedAt": "2026-11-09T10:15:30.123Z"
}
```

| Alan | Tip | Açıklama |
|---|---|---|
| messageId | uuid | Mesaj kimliği; idempotency anahtarı |
| envelopeId | uuid | Zarf kimliği |
| eventType | enum | `invoice-created \| envelope-packaged \| envelope-sent \| envelope-status` |
| vkn | string(10) | Vergi kimlik no; partition/bağlantı anahtarı |
| invoiceCount | int | Zarf içindeki fatura sayısı (≤ 20) |
| payloadSha256 | string(64) | Yük bütünlüğü |
| issuedAt | ISO 8601 | Yayın zaman damgası |

Benchmark sabitleri: yük boyutu ≈ 10 KB/mesaj; zarf başına ≤ 20 fatura (GİB kılavuzu).

### 3.1 C# eşlemesi

`EfaturaMesaj` (Shared.Contracts): `MessageId`, `EnvelopeId`, `OlayTipi` (enum `EfaturaOlayTipi`), `Vkn`, `FaturaSayisi`, `PayloadSha256`, `OlusturmaZamani`. Serileştirme JSON; enum → sözleşme dizisi `InvoiceCreated → "invoice-created"` eşlemesi transport katmanında yapılır.

Transport arayüzleri: `IMessagePublisher.PublishAsync(topic, mesaj, ct) → bool`; `IMessageConsumer.SubscribeAsync(topic, handler, ct)`; handler `true` → onay (ack/commit), `false` → tekrar/DLQ.

### 3.2 Topic/kuyruk haritası

| Kanal | RabbitMQ | Kafka |
|---|---|---|
| Olay akışı | topic exchange `efatura.events` | topic `efatura-events` (3 partition, RF=1) |
| Zarf akışı | topic exchange `efatura.zarf` | topic `efatura-zarf` (3 partition, RF=1) |
| ölü harf | queue `efatura.dlq` (+ DLX) | topic `efatura-dlq` |

## 4. GibSimulator SOAP Sözleşmesi (WSDL Taslağı)

- Protokol: SOAP 1.1, WS-I Basic Profile; hedef ad alanı `urn:gov:gib:efatura:dev`; uc nokta `http://gibsimulator:8080/Service.asmx` (SoapCore).
- Hata enjeksiyonu ortam değişkenleri: `SIM_ERROR_RATE` (0–1), `SIM_DELAY_MIN_MS`, `SIM_DELAY_MAX_MS`, `SIM_TIMEOUT_RATE`.
- SOAP Fault temsili: kılavuz hata kodlarından (belge format, gönderici yetki) örneklenir.

| Operasyon | Girdi | Çıktı |
|---|---|---|
| SendInvoice | EnvelopeRequest (envelopeXml) | EnvelopeResult { envelopeId, status } |
| GetInvoiceStatus | StatusRequest (envelopeId) | StatusResult { status: Accepted \| Rejected \| Pending } |
| SendApplicationResponse | ApplicationResponseRequest | AckResult |

WSDL taslağı:

```xml
<?xml version="1.0" encoding="utf-8"?>
<definitions xmlns="http://schemas.xmlsoap.org/wsdl/"
             xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/"
             xmlns:tns="urn:gov:gib:efatura:dev"
             targetNamespace="urn:gov:gib:efatura:dev"
             name="GibInvoiceService">
  <types>
    <xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                targetNamespace="urn:gov:gib:efatura:dev"
                elementFormDefault="qualified">
      <xsd:element name="SendInvoice">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="envelopeXml" type="xsd:string"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:element name="SendInvoiceResponse">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="envelopeId" type="xsd:string"/>
            <xsd:element name="status" type="tns:EnvelopeStatusType"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:element name="GetInvoiceStatus">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="envelopeId" type="xsd:string"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:element name="GetInvoiceStatusResponse">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="status" type="tns:DocumentStatusType"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:element name="SendApplicationResponse">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="applicationResponseXml" type="xsd:string"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:element name="SendApplicationResponseResponse">
        <xsd:complexType>
          <xsd:sequence>
            <xsd:element name="ackId" type="xsd:string"/>
          </xsd:sequence>
        </xsd:complexType>
      </xsd:element>
      <xsd:simpleType name="EnvelopeStatusType">
        <xsd:restriction base="xsd:string">
          <xsd:enumeration value="Sent"/>
          <xsd:enumeration value="Rejected"/>
        </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="DocumentStatusType">
        <xsd:restriction base="xsd:string">
          <xsd:enumeration value="Accepted"/>
          <xsd:enumeration value="Rejected"/>
          <xsd:enumeration value="Pending"/>
        </xsd:restriction>
      </xsd:simpleType>
    </xsd:schema>
  </types>
  <message name="SendInvoiceSoapIn"><part name="parameters" element="tns:SendInvoice"/></message>
  <message name="SendInvoiceSoapOut"><part name="parameters" element="tns:SendInvoiceResponse"/></message>
  <message name="GetInvoiceStatusSoapIn"><part name="parameters" element="tns:GetInvoiceStatus"/></message>
  <message name="GetInvoiceStatusSoapOut"><part name="parameters" element="tns:GetInvoiceStatusResponse"/></message>
  <message name="SendApplicationResponseSoapIn"><part name="parameters" element="tns:SendApplicationResponse"/></message>
  <message name="SendApplicationResponseSoapOut"><part name="parameters" element="tns:SendApplicationResponseResponse"/></message>
  <portType name="GibInvoicePortType">
    <operation name="SendInvoice"><input message="tns:SendInvoiceSoapIn"/><output message="tns:SendInvoiceSoapOut"/></operation>
    <operation name="GetInvoiceStatus"><input message="tns:GetInvoiceStatusSoapIn"/><output message="tns:GetInvoiceStatusSoapOut"/></operation>
    <operation name="SendApplicationResponse"><input message="tns:SendApplicationResponseSoapIn"/><output message="tns:SendApplicationResponseSoapOut"/></operation>
  </portType>
  <binding name="GibInvoiceBinding" type="tns:GibInvoicePortType">
    <soap:binding style="document" transport="http://schemas.xmlsoap.org/soap/http"/>
    <operation name="SendInvoice">
      <soap:operation soapAction="urn:gov:gib:efatura:dev/SendInvoice"/>
      <input><soap:body use="literal"/></input><output><soap:body use="literal"/></output>
    </operation>
    <operation name="GetInvoiceStatus">
      <soap:operation soapAction="urn:gov:gib:efatura:dev/GetInvoiceStatus"/>
      <input><soap:body use="literal"/></input><output><soap:body use="literal"/></output>
    </operation>
    <operation name="SendApplicationResponse">
      <soap:operation soapAction="urn:gov:gib:efatura:dev/SendApplicationResponse"/>
      <input><soap:body use="literal"/></input><output><soap:body use="literal"/></output>
    </operation>
  </binding>
</definitions>
```

## 5. Broker Dengeleme Parametreleri (Adil Karşılaştırma)

| Parametre | RabbitMQ | Kafka |
|---|---|---|
| Kalıcılık | durable queue + persistent mesaj | diske yazım varsayılan ayar |
| Yayın onayı | publisher confirms | `acks=all`, `enable.idempotence=true` |
| Tüketim onayı | manual ack (işleme sonrası) | manual commit (işleme sonrası) |
| Eşzamanlılık | `prefetch=100` | 3 partition + `max.poll.records=100` |
| Sıralama | kuyruk başına | partition başına (partition key = VKN) |
| Sıkıştırma | yok | yok |
| Tekrar/ redelivery | DLX + üstel geri çekilme (1/2/4 sn, 3 tekrar) | replay + uygulama katmanı üstel geri çekilme (aynı dizi) |
| Şema | JSON, ortak sözleşme (Bölüm 3) | JSON, ortak sözleşme (Bölüm 3) |

İstemci kütüphaneleri: `RabbitMQ.Client`, `Confluent.Kafka` (ham istemciler; broker-üstü soyutlama yok).

## 6. Güvenilirlik Desenleri

- **Transactional Outbox (polling publisher):** `outbox(id, message_id, payload, status, created_at, published_at)`; `BackgroundService`, 1 sn yoklama; `SELECT ... FOR UPDATE SKIP LOCKED` ile satır kilidi.
- **Idempotent Consumer:** `processed_messages(message_id PK)`; insert-or-ignore; çift işlenme sayacı metrik olarak raporlanır.
- **DLQ:** `efatura.dlq`; tekrar limiti aşılan mesajlar buraya taşınır; inceleme log üzerinden.
- **Retry (SOAP çağrısı):** Polly üstel geri çekilme (100 ms × 2ⁿ, jitter, maksimum 5 deneme).
- **Circuit Breaker:** Polly; GibAdapter → GibSimulator çağrısında; D3'te davranış gözlemlenir.

## 7. Ölçüm Tanımları

| Metrik | Tanım |
|---|---|
| E2E gecikme | `final_ts − accept_ts` (REST kabul → final DB state) |
| Verim | Saniye başına final state'e ulaşan fatura |
| Toparlanma süresi | Broker yeniden başlatma → backlog tam boşalma |
| Kayıp | Beklenen fatura sayısı − final state'te olan sayı |
| Çift işlenme | `processed_messages` tablosunda aynı `message_id`'ye ikinci insert girişim sayısı |
| DLQ oranı | DLQ'ya düşen mesaj / toplam mesaj |

Zaman damga noktaları: `accept_ts`, `queue_publish_ts`, `consume_ts`, `gib_send_ts`, `final_ts`.

## 8. Ortam Değişkenleri

| Değişken | Kapsam | Değerler |
|---|---|---|
| `SYNC_MODE` | Gateway | `true` (senkron hat) / `false` (asenkron) |
| `BROKER_TYPE` | tüm broker tüketicisi/yayıcısı | `rabbitmq` / `kafka` |
| `SIM_ERROR_RATE` | GibSimulator | 0–1 (varsayılan 0) |
| `SIM_DELAY_MIN_MS` / `SIM_DELAY_MAX_MS` | GibSimulator | gecikme aralığı (D3: 200–800) |
| `SIM_TIMEOUT_RATE` | GibSimulator | 0–1 |
| `PG_*` | servislere göre | bağlantı dizesi parçaları |

## 9. Paylaşılan Kod Yapısı (uye edilmiş)

```
src/Shared.Contracts/
  EfaturaMesaj.cs      DTO + EfaturaOlayTipi enum
  Transport.cs         IMessagePublisher, IMessageConsumer, MesajHandler
```

Sonraki işler: RabbitMQ ve Kafka transport implementasyonları (Hafta 3–4), SoapCore uç noktası (Hafta 2), EF Core kalıcılık katmanı (Hafta 2).
