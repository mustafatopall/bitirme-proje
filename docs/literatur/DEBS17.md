# DEBS17 — Kafka versus RabbitMQ: A Comparative Study

## Künye (IEEE biçimi)

P. Dobbelaere ve K. S. Sheykh Esmaili, "Kafka versus RabbitMQ: A comparative study of two industry reference publish/subscribe implementations," *Proceedings of the 11th ACM International Conference on Distributed and Event-based Systems (DEBS)*, 2017. arXiv:1709.00333.

## Yöntem

İki endüstriyel referans gerçekleştirim (Apache Kafka ve RabbitMQ), iki ayrı senaryoda karşılaştırılır: (1) *broadcaster* senaryosu — tek yayıcıdan çok tüketiciye yüksek hacimli yayın; (2) *case study* senaryosu — sektör mimarisi bağlamında gerçekçi iş akışı. Ölçütler: tepe ve ortalama verim, gecikme, bellek etkisi. Yapılandırma, araçların belgelenmiş varsayılanlarına yakın tutulur; hipotez, araç ile senaryo etkileşiminin varlığıdır: bir teknolojinin üstünlüğü senaryoya bağlıdır.

## Temel Bulgular

- Tek bir "her koşulda kazanan" yok: iki aracın davranışı senaryo ve yapılandırmaya göre belirgin biçimde ayrışır.
- Broadcaster senaryosunda Kafka daha yüksek tepe verim değerlerine ulaşır.
- Case study senaryosunda nispeten düşük hız aralıklarında RabbitMQ'nun gecikme davranışı korunur; hız arttıkça fark daralır ya da tersine döner.
- Yapılandırma kararları ölçümler üzerinde büyük pay taşır: izole broker ölçümü bile titiz deney tasarımı ister.

## Tezdeki Rolü

Karşılaştırma çerçevesinin doğrudan öncülü: senaryo temelli yöntem, SOAP köprüsü ve iş kuralları içeren e-Fatura hattına taşınır. D1 deney metodolojisinin kaynağı; tartışmada seçim karar tablosunun temeli. "Senaryoyu bağımsız değişken değil sabit tut" ilkesi bu çalışmadan gelir.

## Eleştiri/Not

- Ölçüm ortamında SOAP tabanlı entegrasyon katmanı ve iş hattı bileşkeni yoktur; etkinin yalnızca broker'a atfı yazılır — tezin boşluk beyanı bu eksiklikten doğar.
- Veri modeli ikili (binary) düzeydedir; JSON sözleşmesi ve zarf kuralları kapsanmaz.
- Künye, elde bulunan PDF kopyası ile 2026-11-03 tarihinde birebir doğrulandı.
