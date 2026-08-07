namespace Cnhtv.Api.Controllers;

using Cnhtv.Domain.Models;
using Cnhtv.Infrastructure.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

[Route("[controller]")]
public class TvConnectionHistoryController(CnhtvDbContext dbContext) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IQueryable<TvConnectionHistory> Get() =>
        dbContext.TvConnectionHistory.AsNoTracking();
}
