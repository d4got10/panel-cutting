using PanelCutting.Extensions;

namespace PanelCutting.Common;

internal readonly record struct Segment(Point Left, Point Right)
{
    public static Segment FromPoints(Point first, Point second)
    {
        (Point left, Point right) = first.X < second.X 
            ? (first, second) 
            : (second, first);

        return new Segment(left, right);
    }

    public bool IsVertical => Left.X.EqualsWithTolerance(Right.X);
    public double MaxY => double.Max(Left.Y, Right.Y);
    public double MinY => double.Min(Left.Y, Right.Y);
    
    public bool ContainsPointWithX(double x) => IsVertical
        ? Left.X.EqualsWithTolerance(x) 
        : Left.X <= x && x <= Right.X;

    public Point GetPointFromX(double x)
    {
        if (!ContainsPointWithX(x))
            throw new Exception();

        if (IsVertical) 
            return new Point(x, 0);
        
        double y = (Right.Y - Left.Y) / (Right.X - Left.X) * (x - Left.X) + Left.Y;
        return new Point(x, y);
    }
}