namespace DockScripter.Domain.Enums;

public enum DockerContainerStatus
{
    NotInitialized,
    Created,
    Initializing,
    Ready,
    Running,
    Stopped,
    Error
}