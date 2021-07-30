using Microsoft.AspNetCore.Mvc;
using PrerenderPlaywright.Clients;
using PrerenderPlaywright.Messages;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Controllers
{
    [ApiController]
    [Route("processes")]
    public class ProcessController
    {
        private readonly IProcessClient processClient;
        public ProcessController(IProcessClient processClient)
        {
            this.processClient = processClient;
        }

        [HttpGet]
        public async Task<ProcessInfo> Get()
        {
            return await processClient.GetProcessesAsync();
        }
    }
}
