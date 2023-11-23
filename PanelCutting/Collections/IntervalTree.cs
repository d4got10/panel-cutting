using System.Numerics;

namespace PanelCutting.Collections;

internal sealed class IntervalTree<T> where T : IComparable<T>, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IMinMaxValue<T>
{
    private IntervalTree(InternalTreeNode<T>? root, List<Interval<T>> intervals, List<int> sortedByBeginnings, List<int> sortedByEndings)
    {
        _root = root;
        _intervals = intervals;
        _sortedByBeginnings = sortedByBeginnings;
        _sortedByEndings = sortedByEndings;
    }

    private readonly InternalTreeNode<T>? _root;
    private readonly List<Interval<T>> _intervals;
    private readonly List<int> _sortedByBeginnings;
    private readonly List<int> _sortedByEndings;

    public static IntervalTree<T> Create(IEnumerable<Interval<T>> intervalsSource)
    {
        List<Interval<T>> intervals = intervalsSource.ToList();
        List<int> indexes = Enumerable.Range(0, intervals.Count).ToList();
        List<int> sortedByBeginnings = indexes.OrderBy(i => intervals[i].Min).ToList();
        List<int> sortedByEndings = indexes.OrderBy(i => intervals[i].Max).ToList();
        return new IntervalTree<T>(CreateRoot(indexes, intervals), intervals, sortedByBeginnings, sortedByEndings);
    }

    private static InternalTreeNode<T>? CreateRoot(List<int> indexes, IReadOnlyList<Interval<T>> intervals)
    {
        if (indexes.Count == 0) 
            return null;

        List<T> endPoints = indexes.Select(index => intervals[index].Max).ToList();
        endPoints.Sort();
        
        T centerValue = endPoints[endPoints.Count / 2];
        var left = new List<int>();
        var right = new List<int>();
        var sortedByBeginnings = new List<int>();
        var sortedByEnds = new List<int>();

        foreach (int index in indexes)
        {
            Interval<T> interval = intervals[index];
            if(interval.Max.CompareTo(centerValue) < 0)
                left.Add(index);
            else if(interval.Min.CompareTo(centerValue) > 0)
                right.Add(index);
            else
            {
                sortedByBeginnings.Add(index);
                sortedByEnds.Add(index);
            }
        }

        
        sortedByBeginnings.Sort((leftIndex, rightIndex) => CompareBeginnings(intervals[leftIndex], intervals[rightIndex]));
        sortedByEnds.Sort((leftIndex, rightIndex) => ReverseCompareEnds(intervals[leftIndex], intervals[rightIndex]));
        
        var root = new InternalTreeNode<T>
        {
            Value = centerValue,
            SortedByBeginnings = sortedByBeginnings,
            SortedByEnds = sortedByEnds,
            Left = CreateRoot(left, intervals),
            Right = CreateRoot(right, intervals)
        };
        return root;
    }

    public List<int> Intersect(T point)
    {
        var results = new List<int>();
        Intersect(_root, point, results);
        return results;
    }

    public List<int> Intersect(Interval<T> interval)
    {
        List<int> first = Intersect(interval.Min);
        List<int> second = Intersect(interval.Max);

        IEnumerable<int> intersectingBeginnings = GetIntersectingBeginnings(interval);
        IEnumerable<int> intersectingEnds = GetIntersectingEnds(interval);

        return first.Concat(second).Concat(intersectingBeginnings).Concat(intersectingEnds).ToHashSet().ToList();
    }

    private void Intersect(InternalTreeNode<T>? root, T point, List<int> results)
    {
        if (root == null) return;
        
        if (point.CompareTo(root.Value) == 0)
        {
            results.AddRange(root.SortedByBeginnings);
        }
        else if (point.CompareTo(root.Value) < 0)
        {
            Intersect(root.Left, point, results);
            foreach (int index in root.SortedByBeginnings)
            {
                Interval<T> interval = _intervals[index];
                if (point.CompareTo(interval.Min) < 0)
                    break;
                
                results.Add(index);
            }
        }
        else
        {
            Intersect(root.Right, point, results);
            foreach (int index in root.SortedByEnds)
            {
                Interval<T> interval = _intervals[index];
                if (point.CompareTo(interval.Max) > 0)
                    break;
                
                results.Add(index);
            }
        }
    }

    private IEnumerable<int> GetIntersectingBeginnings(Interval<T> interval)
    {
        int left = 0;
        int right = _sortedByBeginnings.Count;
        while (left < right)
        {
            int middle = (right + left) / 2;
            if (_intervals[_sortedByBeginnings[middle]].Min.CompareTo(interval.Min) < 0)
            {
                left = middle + 1;
            }
            else
            {
                right = middle;
            }
        }
        int min = left;
        
        left = 0;
        right = _sortedByBeginnings.Count;
        while (left < right)
        {
            int middle = (right + left) / 2;
            if (_intervals[_sortedByBeginnings[middle]].Min.CompareTo(interval.Max) < 0)
            {
                left = middle + 1;
            }
            else
            {
                right = middle;
            }
        }
        int max = left - 1;
        
        return Enumerable.Range(min, max - min + 1).Select(i => _sortedByBeginnings[i]);
    }

    private IEnumerable<int> GetIntersectingEnds(Interval<T> interval)
    {
        int left = 0;
        int right = _sortedByEndings.Count;
        while (left < right)
        {
            int middle = (right + left) / 2;
            if (_intervals[_sortedByEndings[middle]].Max.CompareTo(interval.Min) < 0)
            {
                left = middle + 1;
            }
            else
            {
                right = middle;
            }
        }
        int min = left;
        
        left = 0;
        right = _sortedByEndings.Count;
        while (left < right)
        {
            int middle = (right + left) / 2;
            if (_intervals[_sortedByEndings[middle]].Max.CompareTo(interval.Max) < 0)
            {
                left = middle + 1;
            }
            else
            {
                right = middle;
            }
        }
        int max = left - 1;
        
        return Enumerable.Range(min, max - min + 1).Select(i => _sortedByEndings[i]);
    }

    private static int CompareBeginnings(Interval<T> lhs, Interval<T> rhs) => lhs.Min.CompareTo(rhs.Min);
    private static int ReverseCompareEnds(Interval<T> lhs, Interval<T> rhs) => rhs.Max.CompareTo(lhs.Max);
}