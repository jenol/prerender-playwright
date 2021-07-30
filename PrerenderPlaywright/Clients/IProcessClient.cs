using PrerenderPlaywright.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public interface IProcessClient
    {
        Task<ProcessInfo> GetProcessesAsync();
    }
}