using Akka.Actor;
using PrerenderPlaywright.Messages;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public class BrowserClient : IBrowserClient
    {
        private readonly IActorRef actorRef;

        public BrowserClient(IActorRef actorRef)
        {
            this.actorRef = actorRef;
        }

        public Task<BrowserInfoResponse> GetBrowserInfoAsync() =>
            actorRef.Ask<BrowserInfoResponse>(new BrowserInfoRequest());
    }
}
