using PanelCutting.Collections;
using PanelCutting.Common;

namespace PanelCutting;

using Interval = Interval<double>;
using IntervalTree = IntervalTree<double>;

public class ProfileCutter
{
    public ProfileCutter(double panelWidth)
    {
        _panelWidth = panelWidth;
    }

    private readonly double _panelWidth;

    public IEnumerable<PanelCut> Cut(Profile profile)
    {
        Point[] vertices = profile.Vertices.ToArray();
        
        Interval profileXRange = GetProfileWidthRange(vertices, out List<Segment> segments);

        IEnumerable<Interval> intervals = segments.Select(ConvertSegmentToInterval);
        IntervalTree intervalTree = IntervalTree.Create(intervals);

        for (Interval panelXRange = Interval.Create(profileXRange.Min, profileXRange.Min + _panelWidth); 
             panelXRange.Min < profileXRange.Max;
             panelXRange = Interval.Create(panelXRange.Max, panelXRange.Max + _panelWidth))
        {
            Interval panelYRange = CalculateYRangeForPanel(intervalTree, panelXRange, segments);

            if(panelYRange.Length > 0)
                yield return new PanelCut(panelYRange.Length);
        }
    }

    private static Interval CalculateYRangeForPanel(
        IntervalTree intervalTree, 
        Interval panelXRange, 
        IReadOnlyList<Segment> segments)
    {
        var panelYRange = Interval.CreateEmpty();

        List<int> intersectingSegments = intervalTree.Intersect(panelXRange);
        foreach (int segmentIndex in intersectingSegments)
        {
            Segment segment = segments[segmentIndex];

            panelYRange = AggregateSegment(panelXRange, segment, panelYRange);
        }

        return panelYRange;
    }

    private static Interval AggregateSegment(Interval panelXRange, Segment segment, Interval panelYRange)
    {
        if (segment.IsVertical)
        {
            panelYRange = panelYRange.AddPoint(segment.MinY);
            panelYRange = panelYRange.AddPoint(segment.MaxY);
            return panelYRange;
        }

        if (segment.ContainsPointWithX(panelXRange.Min))
        {
            Point point = segment.GetPointFromX(panelXRange.Min);
            panelYRange = panelYRange.AddPoint(point.Y);
        }

        if (segment.ContainsPointWithX(panelXRange.Max))
        {
            Point rightPoint = segment.GetPointFromX(panelXRange.Max);
            panelYRange = panelYRange.AddPoint(rightPoint.Y);
        }

        if (panelXRange.Contains(segment.Left.X))
        {
            panelYRange = panelYRange.AddPoint(segment.Left.Y);
        }

        if (panelXRange.Contains(segment.Right.X))
        {
            panelYRange = panelYRange.AddPoint(segment.Right.Y);
        }

        return panelYRange;
    }

    private static Interval GetProfileWidthRange(IReadOnlyList<Point> vertices, out List<Segment> segments)
    {
        segments = new List<Segment>();
        Point currentPoint = vertices[0];
        Interval xRange = Interval.CreateFromPoint(currentPoint.X);
        for (int i = 0; i < vertices.Count; i++)
        {
            Point next = vertices[(i + 1) % vertices.Count];
            segments.Add(Segment.FromPoints(currentPoint, next));
            xRange = xRange.AddPoint(next.X);
            currentPoint = next;
        }

        return xRange;
    }

    private static Interval ConvertSegmentToInterval(Segment s) => Interval.CreateFromPoints(s.Left.X, s.Right.X);
}