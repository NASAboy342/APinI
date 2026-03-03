using System;

namespace APinI.Models;

public class GetLastGitPilot3ReleaseInfoResponse
{
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string ReleaseNotes { get; set; } = string.Empty;
    public string MacOsDownloadUrl { get; set; } = string.Empty;
    public string WindowsDownloadUrl { get; set; } = string.Empty;
    public string LinuxDownloadUrl { get; set; } = string.Empty;
}