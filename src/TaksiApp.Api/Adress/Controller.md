# Address — Controller

## Genel Bakış

`PassengerAddressesController`, giriş yapmış yolcunun (passenger) kayıtlı adreslerini yönetir. Postman koleksiyonundaki **"Saved Addresses"** sözleşmesine göre 4 operasyon sağlar: listeleme, ekleme, güncelleme, silme.

## Sınıf Tanımı

Namespace : TaksiApp.Api.Controllers
Route     : [Route("passenger/addresses")]
Attributes: [ApiController], [SupportedLocale]
Base      : ControllerBase

**Constructor Injection:**

| Alan | Tip | Amaç |
|---|---|---|
| `_addressService` | `IAddressService` | Adres iş kuralları (limit, duplicate title, sahiplik) |
| `_currentPassenger` | `ICurrentPassenger` | İstek sahibi yolcunun kimliğini taşır |

## Yetkilendirme

- **Rol:** Passenger
- **Header:** `Authorization: Bearer {passengerAccessToken}`
- Backend, token'ın yalnızca geçerliliğini değil **rolünü** ve **kaynağa (adrese) sahipliğini** de doğrulamalı.

## Ortak Header Kuralları

| Header | Zorunlu | Kapsam | Not |
|---|---|---|---|
| `Locale` | Evet | Tüm endpointler | Desteklenmeyen değerde `422 UNSUPPORTED_LOCALE` |
| `Content-Type: application/json` | Evet | Body gönderen istekler (POST/PATCH) | |
| `Idempotency-Key` | Evet | Yalnızca `POST` (Add Address) | Aynı retry aynı key, yeni işlem yeni key |

## Action Metodları

| Metod adı | HTTP | Route | Dönüş tipi |
|---|---|---|---|
| `GetAddresses` | GET | `/passenger/addresses` | `Task<IActionResult>` |
| `AddAddress` | POST | `/passenger/addresses` | `Task<IActionResult>` |
| `UpdateAddress` | PATCH | `/passenger/addresses/{addressId}` | `Task<IActionResult>` |
| `DeleteAddress` | DELETE | `/passenger/addresses/{addressId}` | `Task<IActionResult>` |

Detaylı header/body/response tabloları → **b-Endpoints.md**

## Response Sözleşmesi

- **Başarı:** `ApiSuccessResponse<T>` → `{ success: true, result: T, message }`
- **Hata:** `ApiErrorResponse` → `{ success: false, error: { code, message, details? } }`
- İstisnalar `ExceptionHandlingMiddleware` (`src/TaksiApp.Api/Common/ExceptionHandlingMiddleware.cs`) tarafından yakalanıp bu formata çevrilir.

## Sorumluluk Sınırları

| Katman | Sorumluluk |
|---|---|
| Controller | Request/response mapping, model validation, HTTP status seçimi |
| `IAddressService` | İş kuralları: adres limiti, duplicate title, sahiplik, "adres kullanımda" kontrolü |
| DTO'lar | → **c-DTOs.md** |

## Dikkat Edilecek Noktalar

- `title` alanı adresler arasında **benzersiz** olmalı (aksi halde `409 DUPLICATE_ADDRESS_TITLE`).
- Aktif bir yolculukta kullanılan adres silinemez (`409 ADDRESS_IN_USE`).
- Maksimum kayıtlı adres sayısı var; aşılırsa `409 ADDRESS_LIMIT_EXCEEDED`.
- `PATCH` (Update) body'sinde tüm alanlar tek tek opsiyonel, ancak body tamamen boş olamaz.