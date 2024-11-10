using System.Security.Claims;
using DockScripter.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DockScripter.Controllers;

public class ControllerUtils
{
    public static Guid GetUserIdFromToken(ControllerBase controller)
    {
        var userIdString = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdString!);
    }

    public static ScriptLanguage ParseLanguageStrToEnum(string language)
    {
        if (Enum.TryParse<ScriptLanguage>(language, true, out var parsedEnum))
        {
            return parsedEnum;
        }
        else
        {
            throw new ArgumentException($"Invalid language: {language}");
        }
    }
}