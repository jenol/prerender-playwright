using PrerenderPlaywright.Messages;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public interface IBrowserClient
    {
        Task<BrowserInfoResponse> GetBrowserInfoAsync();
    }
}