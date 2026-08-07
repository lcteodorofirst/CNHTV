namespace Cnhtv.Application.Common;

public record AppConfiguration
{
    public string AppDbConnection { get; init; } = string.Empty;
}
