namespace Cnhtv.Api.Controllers;

using Cnhtv.Application.Common;
using Cnhtv.Application.Dtos.TvDevice;
using Cnhtv.Domain.Models;
using Cnhtv.Infrastructure.DataContext;
using Cnhtv.Application.Interfaces.Devices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

[Route("[controller]")]
public class TvDeviceController(
    CnhtvDbContext dbContext,
    ITvDeviceService tvDeviceService) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IQueryable<TvDevice> Get() => dbContext.TvDevices.AsNoTracking();

    [HttpGet("{key:long}")]
    [EnableQuery]
    public SingleResult<TvDevice> Get(long key) =>
        SingleResult.Create(dbContext.TvDevices.AsNoTracking().Where(x => x.Id == key));

    [HttpPatch("{key:long}")]
    public async Task<IActionResult> Patch(
        long key,
        [FromBody] TvDeviceUpdateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var device = await tvDeviceService.UpdateAsync(key, dto, cancellationToken);
            if (device is null)
                return NotFound();
            return Ok(device);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
