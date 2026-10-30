namespace StarShip.Domain.Models.Shared;

public record Coordinate(int X, int Y)
{
    public override string ToString() => $"({X},{Y}) Lichtjahre";

    public double DistanceTo(Coordinate other)
    {
        ArgumentNullException.ThrowIfNull(other);
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}