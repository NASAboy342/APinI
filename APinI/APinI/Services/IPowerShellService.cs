using APinI.Models;

namespace APinI.Services
{
    public interface IPowerShellService
    {
        Task<string> RunPowerShellScript(PowerShellRequest request);
    }
}
