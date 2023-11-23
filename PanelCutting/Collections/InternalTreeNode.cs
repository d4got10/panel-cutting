using System.Numerics;

namespace PanelCutting.Collections;

internal sealed class InternalTreeNode<T> where T : IComparable<T>, ISubtractionOperators<T, T, T>, IMinMaxValue<T>
{
    public required T Value { get; set; }
    public required List<int> SortedByBeginnings { get; set; }
    public required List<int> SortedByEnds { get; set; }
    public InternalTreeNode<T>? Left { get; set; }
    public InternalTreeNode<T>? Right { get; set; }
}