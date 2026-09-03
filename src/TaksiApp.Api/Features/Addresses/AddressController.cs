using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Features.Addresses.DTOs;
using TaksiApp.Api.Features.Addresses.Services;

namespace TaksiApp.Api.Features.Addresses;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses()
    {
        var addresses = await _addressService.GetUserAddressesAsync();
        return Ok(addresses);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] CreateAddressRequest request)
    {
        var address = await _addressService.CreateAddressAsync(request);
        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, address);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request)
    {
        var address = await _addressService.UpdateAddressAsync(id, request);
        return Ok(address);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAddress(Guid id)
    {
        await _addressService.DeleteAddressAsync(id);
        return NoContent();
    }
}