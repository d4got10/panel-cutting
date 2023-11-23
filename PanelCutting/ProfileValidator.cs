using System.ComponentModel.DataAnnotations;

namespace PanelCutting;

internal static class ProfileValidator
{
    public static bool Validate(Profile profile, out ValidationException? exception)
    {
        if (profile.Vertices.Count < 3)
        {
            exception = new ValidationException("Not enough verticies in profile");
            return false;
        }

        //Other validation logic

        exception = null;
        return true;
    }
}