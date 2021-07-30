using Akka.Actor;
using Microsoft.Playwright;
using Newtonsoft.Json;
using PrerenderPlaywright.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Actors
{
    public class PrerenderActor : ReceiveActor
    {
        private IPlaywright playwright;
        private IBrowser browser;

        private readonly IActorRef activityObserverRef;
        public PrerenderActor(IActorRef activityObserverRef)
        {
            this.activityObserverRef = activityObserverRef;

            ReceiveAsync((Func<RenderingRequestMessage, Task>)(async message => {
                var id = Guid.NewGuid();

                IBrowserContext context = null;
                IPage page = null;

                try
                {
                    var windowSize = message.IsMobile ? new Size(480, 320) : new Size(1920, 1200);
                    var capabilities = new Dictionary<string, object>
                    {
                        ["window-size"] = $"{windowSize.Width},{ windowSize.Height}"
                    };

                    if (playwright == null)
                    {
                        playwright = await Playwright.CreateAsync();
                    }

                    if (browser == null)
                    {
                        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                        {
                            Headless = true
                        });

                        activityObserverRef.Tell(new BrowserCreatedMessage(
                           Thread.CurrentThread.ManagedThreadId,
                           capabilities));
                    }                    

                    context = await browser.NewContextAsync();

                    var redirects = new BlockingCollection<string>();
                    IResponse mainResponse = null;
                    var mainUrl = message.Url.ToLower().TrimEnd('/');
                    EventHandler<IResponse> p = (_, response) =>
                    {
                        var responseUrl = response.Url.ToLower().TrimEnd('/');                        

                        if (responseUrl == mainUrl)
                        {
                            mainResponse = response;
                            if (mainResponse.Headers.TryGetValue("location", out var location))
                            {
                                mainUrl = location.ToLower().TrimEnd('/');
                                redirects.Add(location);
                            }
                        }        
                    };

                    context.Response += p;

                    page = await context.NewPageAsync();
                    await page.SetViewportSizeAsync(windowSize.Width, windowSize.Height);

                    activityObserverRef.Tell(new RenderingStartMessage(
                        id,
                        -1,
                        Thread.CurrentThread.ManagedThreadId,
                        message.StartDate,
                        capabilities));

                    var timer = Stopwatch.StartNew();                    

                    await page.GotoAsync(message.Url);                                      
                    await page.WaitForPrerenderReady();                   

                    context.Response -= p;

                    string html = null;
                    byte[] screenShot = null;
                    string redirectUrl = null;
                    var status = 200;

                    if (redirects.Any())
                    {
                        redirectUrl = redirects.ToArray()[redirects.Count - 1];
                        status = 301;
                    }
                    else
                    {
                        status = mainResponse?.Status ?? 200;

                        if (status == 200)
                        {
                            if (message.Activity == Messages.Activity.RenderHtml)
                            {
                                html = await page.ContentAsync();
                            }
                            else
                            {
                                screenShot = await page.ScreenshotAsync();
                            }
                        }
                    }
                    
                    activityObserverRef.Tell(new RenderingEndMessage(id, status, timer.Elapsed));
                    Sender.Tell(new RenderingResponseMessage(
                        Self.Path.ToStringWithAddress(), html, screenShot, status, redirectUrl));
                }
                catch (Exception ex)
                {
                    Sender.Tell(new RenderingResponseMessage(
                        Self.Path.ToStringWithAddress(), JsonConvert.SerializeObject(ex), null, 500, null));
                }
                finally
                {
                    await page.CloseAsync();
                    await context.CloseAsync();
                }
            }));
        }
       
        public override void AroundPostStop()
        {
            browser?.DisposeAsync();
            playwright?.Dispose();
            base.AroundPostStop();
        }

        public static Props Props(IActorRef activityObserverRef)
        {
            return Akka.Actor.Props.Create(() => new PrerenderActor(activityObserverRef));
        }
    }
}
