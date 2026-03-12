using System;
using APinI.Enums.GitPilot3;

namespace APinI.Models;

public class CountClickRequest
{
    public EnumGitPilot3ClickType ClickType { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Fingerprint { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string ClickTypeString => ClickType.ToString();
}
