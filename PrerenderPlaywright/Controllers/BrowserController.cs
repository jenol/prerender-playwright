using Microsoft.AspNetCore.Mvc;
using PrerenderPlaywright.Clients;
using PrerenderPlaywright.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Controllers
{
    [ApiController]
    [Route("browsers")]
    public class BrowserController: ControllerBase
    {
        private readonly IBrowserClient browserClient;
        public BrowserController(IBrowserClient browserClient)
        {
            this.browserClient = browserClient;
        }

        [HttpGet]
        public async Task<IEnumerable<BrowserInfo>> Get()
        {
            var response = await browserClient.GetBrowserInfoAsync();
            return response.Browsers;
        }
    }
}
