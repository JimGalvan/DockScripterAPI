namespace DockScripter.Domain.Enums;

public enum DockerContainerStatus
{
    NonCreated,
    Created,
    Running,
    Paused,
    Restarting,
    Exited,
    Dead
}