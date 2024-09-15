namespace Domain.Common;

public class BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime TimeStap { get; } = DateTime.Now;
}