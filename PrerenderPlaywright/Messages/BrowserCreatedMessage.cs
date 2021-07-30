using System;
using System.Collections.Generic;

namespace PrerenderPlaywright.Messages
{
    public class BrowserCreatedMessage
    {
        public BrowserCreatedMessage(int threadId, IReadOnlyDictionary<string, object> capabilities)
        {
            ThreadId = threadId;
            Capabilities = capabilities;
        }

        public Guid Id { get; }
        public int ThreadId { get; }
        public IReadOnlyDictionary<string, object> Capabilities { get; }
    }
}
