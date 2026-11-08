# RICH19 — Microservices Patterns

## Künye

C. Richardson, *Microservices Patterns: With examples in Java*. Manning, 2019.

## Yöntem

Desen odaklı mühendislik metni; mikroservis mimarisinde veri tutarlılığı ve entegrasyon desenleri, ardışık örnek uygulamalar ile anlatılır. Her desen için problem, çözüm, artı/eksi analizi sunar.

## Temel Bulgular (desenler)

- **Transactional Outbox:** veritabanı işlemiyle mesaj yayınının atomikliği; polling publisher veya CDC ile outbox tablosundan yayınlama.
- **Idempotent Consumer:** mesajın birden çok kez işlemesini doğru sonuç üretmek; mesaj kimliği tablosu ile çift işlenme engeli.
- **Saga:** dağıtık işlemlerin yerel işlemler + kompensasyon ile yönetimi (tez kapsamı dışında bırakılır).

## Tezdeki Rolü

- RQ2 desen setinin (Outbox + Idempotent Consumer) birincil referansı; Bölüm 3 tasarım betimlemelerinin dayanağı.

## Eleştiri/Not

- Örnekler Java ekosisteminden; desenler teknoloji bağımsız. Polling publisher aralığı ile ilgili kesin koşum sayısı belirtilmez; tezde 1 sn yoklama parametre olarak ele alınır.
