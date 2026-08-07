namespace Cnhtv.Api.Controllers;

using Cnhtv.Application.Dtos.TvDevice;
using Cnhtv.Application.Interfaces.Devices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[EnableCors("DisplayCors")]
[Route("display")]
public class DisplayController(ITvDeviceService tvDeviceService) : ControllerBase
{
    [HttpPost("connect")]
    public async Task<ActionResult<TvDeviceConfigurationDto>> Connect(
        TvDeviceConnectDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.DeviceKey))
            return BadRequest(new { message = "DeviceKey é obrigatório." });

        var result = await tvDeviceService.ConnectAsync(
            dto,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{deviceKey}/heartbeat")]
    public async Task<ActionResult<TvDeviceConfigurationDto>> Heartbeat(
        string deviceKey,
        TvDeviceHeartbeatDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tvDeviceService.HeartbeatAsync(
            deviceKey,
            dto,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
