namespace ParkOnyx.Domain.Helpers;

public static class SwaggerDefaultValuesGenerator
{
    public static string GetRandomEmail()
    {
        return $"test{new Random().Next(1, 1000)}@test.com";
    }
}