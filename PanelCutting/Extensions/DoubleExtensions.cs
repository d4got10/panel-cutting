namespace PanelCutting.Extensions;

internal static class DoubleExtensions
{
    public const double EqualityTolerance = 0.0000001;
    
    public static bool EqualsWithTolerance(this double self, double other) =>
        Math.Abs(self - other) < EqualityTolerance;
}