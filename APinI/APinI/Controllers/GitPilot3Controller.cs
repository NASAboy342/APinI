using System;
using APinI.Enums.GitPilot3;
using APinI.Models;
using APinI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GitPilot3Controller : ControllerBase
{
    private readonly IClickCountService _clickCountService;
    public GitPilot3Controller(IClickCountService clickCountService)
    {
        _clickCountService = clickCountService;
    }
    [HttpGet]
    public GetLastGitPilot3ReleaseInfoResponse GetLastGitPilot3ReleaseInfo()
    {
        return new GetLastGitPilot3ReleaseInfoResponse
        {
            Version = "v1.0.2",
            ReleaseDate = new DateTime(2026, 3, 18),
            ReleaseNotes = "GitPilot v1.0.2 is the latest release.",
            MacOsDownloadUrl = "https://github.com/NASAboy342/GitPilot3/releases/download/v1.0.2/GitPilot3.dmg",
            WindowsDownloadUrl = "https://github.com/NASAboy342/GitPilot3/releases/download/v1.0.1/GitPilot3Setup.exe",
            LinuxDownloadUrl = ""
        };
    }

    [HttpGet]
    public async Task<GetClickCountsResponse> GetClickCounts(string site = "")
    {
        try
        {
            _clickCountService.ValidateSiteName(site);
            var tasks = new List<Task<int>>
            {
                Task.Run(() => _clickCountService.GetClickCounts(site, EnumGitPilot3ClickType.MacOsDownload)),
                Task.Run(() => _clickCountService.GetClickCounts(site, EnumGitPilot3ClickType.WindowsDownload)),
                Task.Run(() => _clickCountService.GetClickCounts(site, EnumGitPilot3ClickType.LinuxDownload)),
                Task.Run(() => _clickCountService.GetClickCounts(site, EnumGitPilot3ClickType.SiteVisit))
            };
            await Task.WhenAll(tasks);
            var macOsDownloadClickCount = tasks[0].Result;
            var windowsDownloadClickCount = tasks[1].Result;
            var linuxDownloadClickCount = tasks[2].Result;
            var siteVisitCount = tasks[3].Result;
            return new GetClickCountsResponse
            {
                MacOsDownloadClickCount = macOsDownloadClickCount,
                WindowsDownloadClickCount = windowsDownloadClickCount,
                LinuxDownloadClickCount = linuxDownloadClickCount,
                SiteVisitCount = siteVisitCount
            };
        }
        catch (Exception ex)
        {
            return new GetClickCountsResponse
            {
                MacOsDownloadClickCount = 0,
                WindowsDownloadClickCount = 0,
                LinuxDownloadClickCount = 0,
                SiteVisitCount = 0,
                ErrorMessage = $"Error retrieving click counts: {ex.Message}"
            };
        }
    }

    [HttpPost]
    public CountClickResponse CountClick(CountClickRequest request)
    {
        try
        {
            _clickCountService.ValidateSiteName(request.Site);
            _clickCountService.CountClick(request);

            return new CountClickResponse
            {
                Success = true,
                Message = "Click counted successfully."
            };
        }
        catch (Exception ex)
        {
            return new CountClickResponse
            {
                Success = false,
                Message = $"Error counting click: {ex.Message}"
            };
        }
    }
}
