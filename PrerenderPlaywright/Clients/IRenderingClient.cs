using PrerenderPlaywright.Messages;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public interface IRenderingClient
    {
        Task<RenderingResponseMessage> RenderAsync(string url, bool isMobile, Activity activity);
    }
}