using WTA.Core.Abstractions;

namespace WTA.Application.Authentication;

[Options]
public class IdentityOptions
{
    public bool SupportsUserLockout { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public TimeSpan DefaultLockoutTimeSpan { get; set; }
}
