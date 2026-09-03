# TaksiApp API — Postman Kılavuzu

## Temel URL
`https://localhost:7247` (development portu `launchSettings.json`'dan kontrol et)

---

## 1️⃣ Auth Akışı

### 📤 **1. Send OTP**
```
POST /api/auth/send-otp
Content-Type: application/json

{
  "phoneNumber": "5551234567"
}
```

✅ Cevap:
```json
{
  "message": "OTP gönderildi."
}
```

### ✅ **2. Verify OTP**
```
POST /api/auth/verify-otp
Content-Type: application/json

{
  "phoneNumber": "5551234567",
  "otpCode": "123456"
}
```

✅ Cevap:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "3sdfg...",
  "isNewUser": true
}
```

### ✍️ **3. Complete Profile** (sadece yeni kullanıcılar için)
```
POST /api/auth/complete-profile
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "fullName": "Ahmet Yılmaz",
  "email": "ahmet@example.com"
}
```

### 🔄 **4. Refresh Token**
```
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "<refreshToken>"
}
```

---

## 2️⃣ Address CRUD

### 📍 **1. Tüm Adreslerimi Getir**
```
GET /api/passenger/addresses
Authorization: Bearer <accessToken>
```

### ➕ **2. Yeni Adres Ekle**
```
POST /api/passenger/addresses
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "title": "Ev",
  "fullAddress": "İstanbul, Kadıköy",
  "latitude": 41.0061,
  "longitude": 29.0279
}
```

### ✏️ **3. Adres Güncelle**
```
PUT /api/passenger/addresses/{addressId}
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "title": "İş",
  "fullAddress": "İstanbul, Beşiktaş",
  "latitude": 41.0434,
  "longitude": 29.0044
}
```

### ❌ **4. Adres Sil**
```
DELETE /api/passenger/addresses/{addressId}
Authorization: Bearer <accessToken>
```

---

## 🧪 Test Data

### İlk Kullanıcı İçin OTP
```json
"phoneNumber": "5551234567"
"otpCode": "123456"  // OtpService.RandomNumberGenerator(100000,999999)
```

### Alternatif Test
```json
"phoneNumber": "5557654321"
"otpCode": "654321"
```

---

## ⚠️ Sorun Giderme

| Belirti | Olası Sebep |
|---------|------------|
| `401 Unauthorized` | Token yok veya süresi dolmuş |
| `400 Bad Request` | OTP süresi dolmuş veya kullanılmış |
| `500 Internal Server Error` | `dotnet run` yap veya migration'ları uygula |

---

## 📝 Sırada Yapılacaklar

1. **SMS Entegrasyonu** — `AuthService.SendOtpAsync()`'de TODO var, Twilio veya başka SMS servisi eklenmeli
2. **Ride Entity'si** — `Ride`, `Driver`, `Payment` modelleri
3. **Driver Endpoint'leri** — `/api/driver/rides`, `/api/driver/location`
4. **Payment Gateway** — Stripe/iyzico entegrasyonu
5. **Real-time Push** — SignalR ile driver-passenger mesajlaşma

---

## 🚀 Hızlı Başlangıç

```bash
# 1. Database oluştur
dotnet ef database update

# 2. Projeyi çalıştır
dotnet run

# 3. Postman'da auth akışını test et
#    send-otp → verify-otp → complete-profile → get-addresses
```