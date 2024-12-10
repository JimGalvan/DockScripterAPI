namespace DockScripter.Services.Interfaces;

public interface IScriptTriggerService
{
    Task SendScriptTriggerAsync(string scriptName, Dictionary<string, string> parameters);
}