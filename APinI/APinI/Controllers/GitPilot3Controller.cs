using System;
using APinI.Models;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GitPilot3Controller : ControllerBase
{
    [HttpGet]
    public GetLastGitPilot3ReleaseInfoResponse GetLastGitPilot3ReleaseInfo()
    {
        return new GetLastGitPilot3ReleaseInfoResponse
        {
            Version = "v1.0.0",
            ReleaseDate = new DateTime(2026, 3, 2),
            ReleaseNotes = "GitPilot v1.0.0 is the initial release.",
            MacOsDownloadUrl = "https://github.com/NASAboy342/GitPilot3/releases/download/v1.0.0/GitPilot3.dmg",
            WindowsDownloadUrl = "",
            LinuxDownloadUrl = ""
        };
    }

    [HttpGet]
    public GetClickCountsResponse GetClickCounts()
    {
        return new GetClickCountsResponse
        {
            MacOsDownloadClickCount = 150,
            WindowsDownloadClickCount = 100,
            LinuxDownloadClickCount = 0,
            SiteVisitCount = 500
        };
    }
}
