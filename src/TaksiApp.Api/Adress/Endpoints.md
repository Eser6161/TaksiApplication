# Address — Endpoints

Tüm endpointler `passenger/addresses` altında, rol: **Passenger**. Kaynak: Postman koleksiyonu "Saved Addresses" klasörü.

---

## 1. GET `/passenger/addresses` — Get Addresses

Giriş yapan yolcunun kayıtlı adreslerini getirir.

**Headers**

| Header | Zorunlu | Değer |
|---|---|---|
| `Authorization` | Evet | `Bearer {passengerAccessToken}` |
| `Locale` | Evet | örn. `tr-TR` |

**Path / Query:** yok. **Body:** yok.

**Responses**

| HTTP | error.code | Açıklama |
|---:|---|---|
| 200 | — | Adresler başarıyla getirildi → `result.addresses: AddressDto[]` |
| 401 | `INVALID_ACCESS_TOKEN` | Access token geçersiz/eksik |
| 403 | `INSUFFICIENT_PERMISSION` | Bu adrese erişim yetkisi yok |
| 429 | `RATE_LIMIT_EXCEEDED` | Çok fazla istek |
| 422 | `UNSUPPORTED_LOCALE` | Desteklenmeyen dil |
| 500 | `INTERNAL_SERVER_ERROR` | Sunucu hatası |

---

## 2. POST `/passenger/addresses` — Add Address

Yolcunun hesabına yeni kayıtlı adres ekler.

**Headers**

| Header | Zorunlu | Değer |
|---|---|---|
| `Authorization` | Evet | `Bearer {passengerAccessToken}` |
| `Locale` | Evet | örn. `tr-TR` |
| `Content-Type` | Evet | `application/json` |
| `Idempotency-Key` | Evet | Benzersiz key (retry'da aynı) |

**Body**

| Alan | Tip | Zorunlu | Kural |
|---|---|---:|---|
| `title` | string | Evet | Kısa, kullanıcıya gösterilir başlık (örn. "Ev", "İş") |
| `addressText` | string | Evet | Boş olamaz |
| `latitude` | number | Evet | -90..90 (WGS84) |
| `longitude` | number | Evet | -180..180 (WGS84) |

**Responses**

| HTTP | error.code | Açıklama |
|---:|---|---|
| 201 | — | Adres eklendi → `result.address: AddressDto` |
| 401 | `INVALID_ACCESS_TOKEN` | Access token geçersiz/eksik |
| 409 | `ADDRESS_LIMIT_EXCEEDED` | Maksimum adres limitine ulaşıldı |
| 409 | `DUPLICATE_ADDRESS_TITLE` | Aynı başlıkla adres zaten var |
| 422 | `VALIDATION_ERROR` | Gönderilen veri geçersiz |
| 503 | `ADDRESS_VALIDATION_SERVICE_UNAVAILABLE` | Dış adres doğrulama servisi kullanılamıyor |
| 422 | `UNSUPPORTED_LOCALE` | Desteklenmeyen dil |
| 400 | `IDEMPOTENCY_KEY_REQUIRED` | Idempotency-Key eksik |
| 409 | `IDEMPOTENCY_KEY_REUSED` | Aynı key farklı payload ile tekrar kullanıldı |
| 500 | `INTERNAL_SERVER_ERROR` | Sunucu hatası |

---

## 3. PATCH `/passenger/addresses/{addressId}` — Update Address

Kayıtlı adresi **kısmi** olarak günceller.

**Headers:** `Authorization`, `Locale`, `Content-Type: application/json`

**Path Params**

| Değişken | Tip | Zorunlu | Kural |
|---|---|---:|---|
| `addressId` | string (ID) | Evet | Yalnızca kendi adresinde kullanılabilir |

**Body** (tüm alanlar tek tek opsiyonel, ama body tamamen boş olamaz)

| Alan | Tip | Kural |
|---|---|---|
| `title` | string | Kısa başlık |
| `addressText` | string | Boş olamaz |
| `latitude` | number | -90..90 |
| `longitude` | number | -180..180 |
| `description` | string | Serbest metin, uzunluk sınırı backend'de |

**Responses**

| HTTP | error.code | Açıklama |
|---:|---|---|
| 200 | — | Adres güncellendi → `result` |
| 404 | `ADDRESS_NOT_FOUND` | Adres bulunamadı |
| 409 | `DUPLICATE_ADDRESS_TITLE` | Aynı başlıkla başka adres var |
| 422 | `VALIDATION_ERROR` | Doğrulama hatası |
| 401 | `INVALID_ACCESS_TOKEN` | Access token geçersiz/eksik |
| 422 | `UNSUPPORTED_LOCALE` | Desteklenmeyen dil |
| 500 | `INTERNAL_SERVER_ERROR` | Sunucu hatası |

---

## 4. DELETE `/passenger/addresses/{addressId}` — Delete Address

Yolcuya ait kayıtlı adresi siler.

**Headers:** `Authorization`, `Locale`

**Path Params**

| Değişken | Tip | Zorunlu | Kural |
|---|---|---:|---|
| `addressId` | string (ID) | Evet | Yalnızca kendi adresinde kullanılabilir |

**Body:** yok

**Responses**

| HTTP | error.code | Açıklama |
|---:|---|---|
| 200 | — | Adres silindi → `result.deletedAddressId` |
| 401 | `INVALID_ACCESS_TOKEN` | Access token geçersiz/eksik |
| 403 | `FORBIDDEN_ADDRESS_DELETE` | Başka kullanıcının adresi |
| 404 | `ADDRESS_NOT_FOUND` | Silinmek istenen adres bulunamadı |
| 409 | `ADDRESS_IN_USE` | Devam eden yolculuğa bağlı, silinemez |
| 422 | `UNSUPPORTED_LOCALE` | Desteklenmeyen dil |
| 500 | `INTERNAL_SERVER_ERROR` | Sunucu hatası |

---

## Genel Kural

Tüm başarı cevapları `success: true` + `result`; tüm hata cevapları `success: false` + `error.code` + `error.message` formatını kullanır. İstemci karar verirken `message` metnine değil, HTTP status + `error.code`'a bakmalı.