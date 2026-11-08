# GIB19 — Türkiye GİB e-Fatura Entegrasyon Kılavuzu

## Künye

Gelir İdaresi Başkanlığı (GİB), *e-Fatura Uygulaması (Entegrasyon Kılavuzu)*, v1.9. ebelge.gib.gov.tr, 2019 (güncel sürüm kontrol edilir).

## İçerik/Yöntem

Özel entegratör modelinde GİB ve entegratörler arasındaki teknik iş alanını tanımlayan resmi belge: SOAP tabanlı web servis sözleşmeleri, zarf ve fatura kuralları, hata kodları ve süreç akışları.

## Temel Tanımlar (tez için kritik)

- SOAP tabanlı üç temel servis kabiliyeti: fatura gönderim (SendInvoice), durum sorgu (GetInvoiceStatus), application response gönderimi (SendApplicationResponse).
- Zarf içerik sınırı: bir zarfta en fazla 20 fatura belgesi.
- Hata kodları: belge format, gönderici yetki gibi kategoriler; SOAP Fault temsili kılavuz kodlarıyla örneklenir.
- Bu yüzden: hat modelinde "kesinti ve hata" davranışlarının kılavuzla uyumluluğu, simülatörün gerekçesidir.

## Tezdeki Rolü

- Alan birincil kaynağı: senaryonun iş kuralları (zarf paketleme, durum akışı), SOAP sözleşmesi ve hata modelinin dayanağı (Bölüm 2–3; simülatör doğrulama).

## Eleştiri/Not

- Resmi belge, akademik atıf değil; her sürüm değişimi sürüm numarasıyla sabitlenir (v1.9). Gerçek GİB entegrasyonu sertifikasyon gerektirir; tezde simülatörle davranışsal eşdeğerlik kullanılır (kapsam dışı gerekçesi tez 1.7).
