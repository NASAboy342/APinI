using System;
using APinI.Enums.GitPilot3;
using APinI.Models;

namespace APinI.Services;

public interface IClickCountService
{
    void CountClick(CountClickRequest request);
    int GetClickCounts(string site, EnumGitPilot3ClickType clickType);
    void ValidateSiteName(string site);
}
