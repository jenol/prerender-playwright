using Akka.Actor;
using PrerenderPlaywright.Messages;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PrerenderPlaywright.Actors
{
    public class BrowserObserverActor: ReceiveActor
    {       
        private readonly Dictionary<string, dynamic> browsers;

        public BrowserObserverActor()
        {
            browsers = new Dictionary<string, dynamic>();

            Receive<BrowserCreatedMessage>(message => {
                UpdateBrowsers(Sender.Path.ToStringWithAddress(), message);
            });

            Receive<RenderingStartMessage>(message => {
                UpdateBrowsers(Sender.Path.ToStringWithAddress(), message);
            });

            Receive<RenderingEndMessage>(message => {
                UpdateBrowsers(Sender.Path.ToStringWithAddress(), message);
            });

            Receive<BrowserInfoRequest>(message => {
                Sender.Tell(GetBrowserInfo());
            });
        }

        private BrowserInfoResponse GetBrowserInfo()
        {
            var infos = from pair in browsers
                        let processId = (int)pair.Value.processId
                        let theadId = (int)pair.Value.theadId
                        let started = (int)pair.Value.started
                        let completed = (int)pair.Value.completed
                        let capabilities = pair.Value.capabilities
                        let lastActivity = (DateTime?)pair.Value.lastActivity
                        select new BrowserInfo(
                            pair.Key,
                            theadId,
                            started,
                            completed,
                            capabilities,
                            lastActivity);

            return new BrowserInfoResponse(infos);
        }       

        private void UpdateBrowsers(string actorName, BrowserCreatedMessage message)
        {
            if (!browsers.TryGetValue(actorName, out var browser))
            {
                browser = new ExpandoObject();
                browsers.Add(actorName, browser);
            }
            
            browser.started = 0;
            browser.completed = 0;
            browser.theadId = message.ThreadId;
            browser.capabilities = message.Capabilities;
            browser.lastActivity = DateTime.Now;
        }

        private void UpdateBrowsers(string actorName, RenderingStartMessage message) 
        {
            if (!browsers.TryGetValue(actorName, out var browser))
            {
                browser = new ExpandoObject();
                browser.started = 0;
                browser.completed = 0;
                browsers.Add(actorName, browser);
            }

            browser.started = ((int)browser.started) + 1;
            browser.processId = message.ProcessId;
            browser.theadId = message.ThreadId;
            browser.capabilities = message.Capabilities;
            browser.lastActivity = (DateTime?)message.StartDate;
        }

        private void UpdateBrowsers(string actorName, RenderingEndMessage message)
        {
            if (!browsers.TryGetValue(actorName, out var browser))
            {
                return;
            }

            var lastActivity = (DateTime?)browser.lastActivity;
            browser.completed = ((int)browser.completed) + 1;
            browser.lastActivity = (DateTime?)(lastActivity.HasValue ? lastActivity.Value.Add(message.Duration) : null);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new BrowserObserverActor());
        }
    }
}
