using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Auth;
using TaksiApp.Api.Common;

namespace TaksiApp.Api.Adress;

[ApiController]
[Authorize(Roles = "Passenger")]
[Route("passenger/addresses")]
public class PassengerAddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly ICurrentPassenger _currentPassenger;

    public PassengerAddressesController(IAddressService addressService, ICurrentPassenger currentPassenger)
    {
        _addressService = addressService;
        _currentPassenger = currentPassenger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        var addresses = await _addressService.GetAddressesAsync(_currentPassenger.PassengerId, cancellationToken);

        return Ok(new ApiResponse<AddressListResult>
        {
            Success = true,
            Result = new AddressListResult { Addresses = addresses },
            Message = "Adresler başarıyla getirildi"
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request, CancellationToken cancellationToken)
    {
        var address = await _addressService.AddAddressAsync(_currentPassenger.PassengerId, request, cancellationToken);

        return StatusCode(201, new ApiResponse<AddAddressResult>
        {
            Success = true,
            Result = new AddAddressResult { Address = address },
            Message = "Adres eklendi"
        });
    }

    [HttpPatch("{addressId}")]
    public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        var address = await _addressService.UpdateAddressAsync(_currentPassenger.PassengerId, addressId, request, cancellationToken);

        return Ok(new ApiResponse<AddressDto>
        {
            Success = true,
            Result = address,
            Message = "Adres güncellendi"
        });
    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> DeleteAddress(Guid addressId, CancellationToken cancellationToken)
    {
        var deletedId = await _addressService.DeleteAddressAsync(_currentPassenger.PassengerId, addressId, cancellationToken);

        return Ok(new ApiResponse<DeleteAddressResult>
        {
            Success = true,
            Result = new DeleteAddressResult { DeletedAddressId = deletedId },
            Message = "Adres silindi"
        });
    }
}