using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaksiApp.Api.Auth;
using TaksiApp.Application.Address;

namespace TaksiApp.Api.Address;

[Authorize]
[ApiController]
[Route("api/passenger/addresses")]
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
    public async Task<IActionResult> GetMyAddresses()
    {
        var result = await _addressService.GetMyAddressesAsync(_currentPassenger.UserId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest request)
    {
        var result = await _addressService.CreateAsync(
            _currentPassenger.UserId, request.Title, request.FullAddress, request.Latitude, request.Longitude);
        return StatusCode(201, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest request)
    {
        var result = await _addressService.UpdateAsync(
            _currentPassenger.UserId, id, request.Title, request.FullAddress, request.Latitude, request.Longitude);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _addressService.DeleteAsync(_currentPassenger.UserId, id);
        return NoContent();
    }
}
