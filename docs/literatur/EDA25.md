# EDA25 — Next-Generation Event-Driven Architectures

## Künye

"Next-Generation Event-Driven Architectures: Performance, Scalability, and Intelligent Orchestration Across Messaging Frameworks," arXiv:2510.04404, 2025.

## Yöntem

Olay güdümlü mimari çerçeveleri ve mesajlama altyapıları (RabbitMQ, Kafka ve diğer akış/ortak mesaj platformları) performans, ölçeklenebilirlik ve orkestrasyon eksenlerinde kıyaslanır. Metodoloji: tekrarlı koşum, ortalama ile birlikte dağılım dilimlerinin raporlanması ve parametrik olmayan anlamlılık testleri (Mann-Whitney U dâhil).

## Temel Bulgular

- Çerçeve seçiminin gecikme ve verim profilleri yük desenine göre değişir; tek ölçütlü "en hızlı framework" bildirimleri yanlış yönlendiricidir.
- Orkestrasyon katmanı ve serileştirme biçimi uçtan uca hattın gecikmesine belirgin katkı yapar.
- Koşum sayısının artırılması ve koşum sırasının rastgeleleştirilmesi gürültü etkisini azaltır.

## Tezdeki Rolü

- D1–D3 deney döngüsünde tekrar sayısı (en az 10 koşum) ve Mann-Whitney U (alfa = 0,05) kararının güncel öncülü.
- "Medyan + çeyrekler arası aralık + etki büyüklüğü" raporlama formatının literatür karşılığı.

## Eleştiri/Not

- Kapsamı geniştir; SOAP kamu entegrasyonu içermez. Yöntemsel kısmı hassas; bulgu tablolarının kendi hattımızda yeniden üretilmesi zorunlu.
