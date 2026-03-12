using System;
using APinI.Enums.GitPilot3;
using APinI.Models;
using APinI.Repository;

namespace APinI.Services;

public class ClickCountService : IClickCountService
{
    private readonly PinDataRepository _pinDataRepository;
    public ClickCountService(PinDataRepository pinDataRepository)
    {
        _pinDataRepository = pinDataRepository;
    }
    public void CountClick(CountClickRequest request)
    {
        _pinDataRepository.CountClick(request);
    }

    public int GetClickCounts(string site, EnumGitPilot3ClickType clickType)
    {
        return _pinDataRepository.GetClickCounts(site, clickType);
    }

    public void ValidateSiteName(string site)
    {
        if (string.IsNullOrEmpty(site))
        {
            throw new ArgumentException("Site name cannot be null or empty.");
        }
    }
}
