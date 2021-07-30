using System;
using System.Collections.Generic;

namespace PrerenderPlaywright.Messages
{
    public class BrowserInfoResponse
    {
        public BrowserInfoResponse(IEnumerable<BrowserInfo> browsers)
        {
            Browsers = browsers;
        }

        public IEnumerable<BrowserInfo> Browsers { get; }
    }

    public class BrowserInfo
    {
        public BrowserInfo(
            string actorName,
            int theadId,
            int started,
            int completed,           
            IReadOnlyDictionary<string, object> capabilities, 
            DateTime? lastActivity)
        {
            ActorName = actorName;
            TheadId = theadId;
            Started = started;
            Completed = completed;
            Capabilities = capabilities;
            LastActivity = lastActivity;
        }

        public string ActorName { get; }
        public int TheadId { get; }
        public int Started { get; }
        public int Completed { get; }
        public IReadOnlyDictionary<string, object> Capabilities { get; }
        public DateTime? LastActivity { get; }
    }    
}
