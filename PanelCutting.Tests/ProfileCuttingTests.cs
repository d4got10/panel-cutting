namespace PanelCutting.Tests;

public class ProfileCuttingTests
{
    private ProfileCutter _profileCutter;
    private const double EqualityPrecision = 0.000001;
    
    [SetUp]
    public void Setup()
    {
        _profileCutter = new ProfileCutter(500);
    }

    [Test]
    public void CutPanels_GivenSquareProfile_ReturnsCorrectCuts()
    {
        var vertices = new List<Point>
        {
            new(0, 0),
            new(0, 1000),
            new(1000, 1000),
            new(1000, 0),
        };

        var profile = Profile.Create(vertices, out _)!;

        List<PanelCut> results = _profileCutter.Cut(profile).ToList();
        
        var expectedResult = new PanelCut[]
        {
            new(1000),
            new(1000),
        };
        
        Assert.That(results, Has.Count.EqualTo(expectedResult.Length));
        Assert.Multiple(() =>
        {
            for (int i = 0; i < expectedResult.Length; i++)
                Assert.That(results[i], Is.EqualTo(expectedResult[i]).Within(EqualityPrecision), $"Results at position {i} differ!");
        });
    }
    
    [Test]
    public void CutPanels_GivenProfileWithFloatingPointCoordinates_ReturnsCorrectCuts()
    {
        var vertices = new List<Point>
        {
            new(0, 0),
            new(0, 62.5),
            new(1000, 250),
            new(1000, 0),
        };

        var profile = Profile.Create(vertices, out _)!;

        List<PanelCut> results = _profileCutter.Cut(profile).ToList();
        
        var expectedResult = new PanelCut[]
        {
            new(156.25),
            new(250),
        };
        
        Assert.That(results, Has.Count.EqualTo(expectedResult.Length));
        Assert.Multiple(() =>
        {
            for (int i = 0; i < expectedResult.Length; i++)
                Assert.That(results[i], Is.EqualTo(expectedResult[i]).Within(EqualityPrecision), $"Results at position {i} differ!");
        });
    }
    
    [Test]
    public void CutPanels_GivenProfileWithMultipleSegmentsInOnePanelInterval_ReturnsCorrectCuts()
    {
        var vertices = new List<Point>
        {
            new(0, 0),
            new(0, 100),
            new(100, 200),
            new(150, 600),
            new(200, 350),
            new(600, 100),
            new(500, 0),
        };

        var profile = Profile.Create(vertices, out _)!;

        List<PanelCut> results = _profileCutter.Cut(profile).ToList();
        
        var expectedResult = new PanelCut[]
        {
            new(600),
            new(162.5),
        };
        
        Assert.That(results, Has.Count.EqualTo(expectedResult.Length));
        Assert.Multiple(() =>
        {
            for (int i = 0; i < expectedResult.Length; i++)
                Assert.That(results[i], Is.EqualTo(expectedResult[i]).Within(EqualityPrecision), $"Results at position {i} differ!");
        });
    }
}