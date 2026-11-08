# HOHP03 — Enterprise Integration Patterns

## Künye

G. Hohpe ve B. Woolf, *Enterprise Integration Patterns: Designing, Building, and Deploying Messaging Solutions*. Addison-Wesley, 2003.

## Yöntem

Kurumsal entegrasyonun desen kataloğu; mesajlamanın (channel, message, router, transformer, endpoint) temel kavram atası desenler biçiminde sistematize edilir.

## Temel Bulgular (desenler)

- **Message Channel / Publish-Subscribe:** bileşenler arası gevşek bağın iletişim temeli.
- **Dead Letter Channel (DLQ):** teslim/işleme başarısızlıklarının izole edilmesi; retry limiti sonrası ele alınış.
- **Competing Consumers:** yük dağılımı için tüketici yarışı; tezin eşzamanlılık yapılandırmasının kavramsal temeli.
- **Idempotent Receiver:** tekrar teslimatın çift işlemesini engelleyen klasik desen.

## Tezdeki Rolü

- DLQ, Idempotent Consumer ve Competing Consumers desen tanımlarının terminolojik kaynağı (Bölüm 2–3).

## Eleştiri/Not

- 2003 yayınlı; modern broker (RabbitMQ/Kafka) gerçekleştirimlerinden bağımsız, kavramsal düzey. Desenlerin broker davranışıyla ölçümlü etkileşimi bu metnin kapsamı dışında kalır — tezin ölçüm katkısı tam buraya değer.
