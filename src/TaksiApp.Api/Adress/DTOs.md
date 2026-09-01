# Address — DTOs

Namespace önerisi: `TaksiApp.Application.Addresses.Dtos`.

## AddressDto (Response — ortak adres modeli)

public class AddressDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string AddressText { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

Kullanıldığı yerler: `GetAddresses` (liste içinde), `AddAddress` (tekil, `result.address`).

## AddressListResult (GetAddresses response wrapper)

public class AddressListResult
{
    public List<AddressDto> Addresses { get; set; }
}

`ApiSuccessResponse<AddressListResult>` içine sarılır.

## AddAddressRequest (POST body)

public class AddAddressRequest
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string AddressText { get; set; }

    [Required, Range(-90, 90)]
    public double Latitude { get; set; }

    [Required, Range(-180, 180)]
    public double Longitude { get; set; }
}

## AddAddressResult (POST response wrapper)

public class AddAddressResult
{
    public AddressDto Address { get; set; }
}

## UpdateAddressRequest (PATCH body — tüm alanlar opsiyonel)

public class UpdateAddressRequest
{
    public string? Title { get; set; }
    public string? AddressText { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Description { get; set; }
}

> Custom validation gerekli: en az bir alan dolu olmalı (hepsi null ise `422 VALIDATION_ERROR`).

## DeleteAddressResult (DELETE response wrapper)

public class DeleteAddressResult
{
    public string DeletedAddressId { get; set; }
}

---

## ⚠️ Sözleşme Tutarsızlıkları (Postman notlarından)

Bunlar backend/mobil ekiple netleştirilmeli, DTO tasarımını etkiler:

1. **`description` alanı sadece `UpdateAddressRequest`'te var** — `AddAddressRequest` ve `AddressDto` (GET/POST response) içinde yok. Yani bir adrese açıklama eklenip sonra listelendiğinde `description` hiçbir zaman geri dönmüyor. DTO'ya `Description` eklenip eklenmeyeceği netleşmeli.
2. Postman notlarında **"Saved Addresses alan modeli gelecekte `pickupLocation`/`dropoffLocation` yapısıyla hizalanacaktır"** deniyor — yani `addressText` + `latitude`/`longitude` düz alanları, ileride nested bir location objesine dönüşebilir. Şimdilik mevcut düz yapı ile gidiyoruz.
3. `409 ADDRESS_IN_USE` senaryosunun kayıtlı örnek adı Postman'da hatalı yazılmış (`"Delete Address"`), gerçek karşılığı `ADDRESS_IN_USE` — DTO'yu etkilemiyor ama backend test/mapping yazarken karışmasın diye not düştüm.