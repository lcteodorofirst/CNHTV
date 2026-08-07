namespace Cnhtv.Domain.Models;

public abstract class BaseEntity<T>
{
    public T Id { get; set; } = default!;
}
