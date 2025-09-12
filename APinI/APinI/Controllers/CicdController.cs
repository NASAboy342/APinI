using APinI.Models;
using APinI.Schedular.Jobs;
using APinI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers
{
    [ApiController]
    [Route("api/CicdController")]
    public class CicdController : ControllerBase
    {
        private readonly ICicdService _cicdService;
        private readonly IPowerShellService _powerShellService;
        
        public CicdController(ICicdService cicdService, IPowerShellService powerShellService)
        {
            _cicdService = cicdService;
            _powerShellService = powerShellService;
        }

        // [HttpPost("update-website")]
        // public async Task<UpdateWebsiteResponse> UpdateWebsite()
        // {
        //     return await _cicdService.UpdateWebsite();
        // }

        // [HttpPost("HackWifi")]
        // public async Task<string> HackWifi(Wifi req)
        // {
        //     WifiCrackingService wifiCrackingService = new WifiCrackingService();
        //     return await wifiCrackingService.ProccessHacking(req);
        // }

        // [HttpGet("update-website-ip")]
        // public string UpdateWebsiteIp()
        // {
        //     try
        //     {
        //         var updateLocalWebsiteIpAddress = new UpdateLocalWebsiteIpAddress();
        //         updateLocalWebsiteIpAddress.ToDo("Go");
        //         return "Success";
        //     }
        //     catch (Exception ex)
        //     {
        //         return ex.ToString();
        //     }
        // }

        // [HttpPost("trigger-api-usage")]
        // public string TriggerApiUsage()
        // {
        //     return "success";
        // }
        
        // [HttpPost("powershell-runner")]
        // public async Task<string> PowerShellRunner(PowerShellRequest request)
        // {
        //     try
        //     {
        //         if (string.IsNullOrEmpty(request.Script))
        //         {
        //             return "Script is empty";
        //         }
        //         return await _powerShellService.RunPowerShellScript(request);
        //     }
        //     catch (Exception ex)
        //     {
        //         return ex.ToString();
        //     }
            
        // }
    }
}
