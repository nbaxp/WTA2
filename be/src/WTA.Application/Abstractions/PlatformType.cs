namespace WTA.Application.Abstractions;

[Flags]
public enum PlatformType
{
    Windows = 0x1,
    Linux = 0x2,
    OSX = 0x4,
    All = Windows | Linux | OSX,
}
