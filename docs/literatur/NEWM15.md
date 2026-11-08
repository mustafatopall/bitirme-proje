# NEWM15 — Building Microservices

## Künye

S. Newman, *Building Microservices: Designing Fine-Grained Systems*. O'Reilly Media, 2015.

## Yöntem

Mikroservis mimarisi tasarımının, disiplinler arası rehberi; servis sınırları, bağımsız dağıtım, veri sahipliği, entegrasyon modelleri ve izleme konuları somut örneklerle işlenir.

## Temel Bulgular (kavramlar)

- Servis sınırlarının iş yetkinliklerine göre çizilmesi; veri tabanının servis başına sahiplenilmesi.
- İstek-tabanlı (senkron) ile olay-tabanlı (asenkron) entegrasyon seçimlerinin tutarlılığı önemsediği ("integration choices make or break your design").
- Bağımsız dağıtım yeteneği mimari değerin önkoşuludur.

## Tezdeki Rolü

- Bileşen sınırlarının (Gateway/FaturaServisi/ZarfServisi/GibAdapter) gerekçelendirilmesi (Bölüm 3); senkron/asenkron seçim sorun alanının çerçevesi.

## Eleştiri/Not

- Yapıtın örnekleri genel; kamu entegrasyonu özelinde yönlendirme yok. Broker seçim ölçütleri sunulur ama nicel karşılaştırma yapılmaz.
