# e-Fatura Mikroservis Karşılaştırma Ortamı

SOAP tabanlı kamu entegrasyonu senaryosunda (e-Fatura özel entegratör modeli), olay güdümlü mikroservis mimarisinde RabbitMQ ve Kafka'nın deneysel karşılaştırması için uygulama deposu.

## Yapı

```
src/Gateway            REST kabul katmanı (202), senkron/asenkron yönlendirme
src/FaturaServisi      UBL-TR üretimi, validasyon, invoice + outbox kalıcılığı
src/ZarfServisi        Zarf paketleme, durum makinesi
src/GibAdapter         SOAP istemci, retry + DLQ yönetimi
src/GibSimulator       GİB SOAP simülatörü (SoapCore), hata enjeksiyonu
src/Shared.Contracts   Mesaj sözleşmesi + transport arayüzleri
tests/SmokeTests       Uçtan uca duman testleri
deploy/                docker-compose + prometheus yapılandırması
docs/literatur         Birincil kaynak notları
docs/design            Mimari tasarım belgesi
data/D1|D2|D3          Deney veri dizinleri
analysis/              Analiz notbook'ları
```

## Durum

- Hafta 1 iskeleti kurulu: çözüm derlenir, duman testleri hazır.
- Simülatör ve servis iş mantığı Hafta 2'de doldurulur.

## Derleme

```
dotnet build EfaturaMikroservis.sln
dotnet test EfaturaMikroservis.sln
```

Ortam değişkenleri (tasarım): `BROKER_TYPE=rabbitmq|kafka`, `SYNC_MODE=true|false`. Simülatör: `SIM_ERROR_RATE`, `SIM_DELAY_MIN_MS`, `SIM_DELAY_MAX_MS`, `SIM_TIMEOUT_RATE`.
