namespace StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Logbooks;

public class BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly BuiltAt { get; init; } = DateOnly.FromDateTime(DateTime.Now);
    public Logbook Logbook { get; }
    public string Name { get; }
    public Coordinate Position { get; set; }

    protected BaseEntity(string name, Coordinate position)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name;
        Position = position?? throw new ArgumentNullException(nameof(position));
        Logbook = new Logbook(this);
    }

    public override string ToString() => Name;

    public double DistanceTo(Coordinate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return Position.DistanceTo(target);
    }

    public double DistanceTo(BaseEntity other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Position.DistanceTo(other.Position);
    }

}