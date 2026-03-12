using System;

namespace APinI.Models;

public class GetClickCountsResponse
{
    public int MacOsDownloadClickCount { get; set; }
    public int WindowsDownloadClickCount { get; set; }
    public int LinuxDownloadClickCount { get; set; }
    public int SiteVisitCount { get; set; }
}
