namespace DockScripter.Core;

public class StringUtils
{
    public static (string Image, string Tag) ParseDockerImage(string dockerImage)
    {
        var parts = dockerImage.Split(':');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid Docker image format. Expected format 'image:tag'.");
        }

        return (parts[0], parts[1]);
    }
}