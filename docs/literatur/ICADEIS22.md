# ICADEIS22 — Event-Driven Architecture to Improve Performance and Scalability

## Künye

"Event-Driven Architecture to Improve Performance and Scalability in Microservices-Based Systems," *ICADEIS*, 2022.

## Yöntem

Mikroservis tabanlı bir sistemde senkron (servisler arası REST çağrısı) ile kuyruk tabanlı eşzamansız iletişim deneysel olarak karşılaştırılır; artan eşzamanlılık seviyelerinde yanıt süreleri ve ölçeklenme davranışı gözlemlenir.

## Temel Bulgular

- Yük arttıkça senkron zincirleme çağrıların gecikmeleri hızla büyür; kuyruk tabanlı hattın gecikme eğrisi belirgin biçimde daha düz ilerler.
- Kuyruk iletişimi ölçeklenmeyi tüketicinin ölçeklenme kapasitesine bağlar; yatıl literatürde EDA'nın performans zaferinin tipik kanıtı niteliğindedir.

## Tezdeki Rolü

- "Senkron SOAP hattı ↔ asenkron broker hattı" karşılaştırmasının (RQ1) doğrudan öncülü.
- H1 hipotezinin literatür dayanağı: asenkron hattın yüksek yükte senkron zincirden üstünlüğü beklentisi.

## Eleştiri/Not

- Senaryo REST dayalıdır; SOAP ve kamu entegrasyonu bağlamı yoktur. SOAP köprüsünün eklenmesi tezin katkı talebini doğrular.
