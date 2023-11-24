using System.ComponentModel.DataAnnotations;
using PanelCutting.Validation;

namespace PanelCutting;

public class Profile
{
    private Profile(List<Point> vertices)
    {
        Vertices = vertices;
    }
    
    public List<Point> Vertices { get; }

    public static Profile? Create(List<Point> vertices, out Exception? exception)
    {
        var profile = new Profile(vertices);
        if (ProfileValidator.Validate(profile, out ValidationException? validationException))
        {
            exception = null;
            return profile;
        }
        
        exception = validationException;
        return null;
    }
}