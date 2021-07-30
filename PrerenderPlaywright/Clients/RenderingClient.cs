using Akka.Actor;
using PrerenderPlaywright.Messages;
using System;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public class RenderingClient : IRenderingClient
    {
        private readonly IActorRef groupRef;

        public RenderingClient(IActorRef groupRef)
        {
            this.groupRef = groupRef;
        }

        public Task<RenderingResponseMessage> RenderAsync(string url, bool isMobile, Activity activity) =>
            groupRef.Ask<RenderingResponseMessage>(new RenderingRequestMessage(url, isMobile, activity, DateTime.Now));
    }
}
