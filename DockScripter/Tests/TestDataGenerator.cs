using System;

namespace DockScripter.Tests
{
    public static class TestDataGenerator
    {
        private static readonly Random Random = new Random();

        public static string GenerateFirstName()
        {
            return $"Test{Random.Next(1000, 9999)}";
        }

        public static string GenerateLastName()
        {
            return $"User{Random.Next(1000, 9999)}";
        }

        public static string GenerateEmail()
        {
            return $"testuser{Random.Next(1000, 9999)}@example.com";
        }

        public static string GenerateScriptName()
        {
            return $"Test Script {Random.Next(1000, 9999)}";
        }

        public static string GenerateScriptDescription()
        {
            return $"A script for testing {Random.Next(1000, 9999)}";
        }

        public static string GenerateEnvironmentName()
        {
            return $"Python Environment {Random.Next(1000, 9999)}";
        }
    }
}