using Microsoft.Playwright;
using System.Threading.Tasks;

namespace PrerenderPlaywright
{
    public static class PageExtensionscs
    {
        public static async Task WaitForPrerenderReady(this IPage page)
        {
            await page.EvaluateAsync(@"
setTimeout(() => { window.prerenderReady = true }, 5000);
if (window.prerenderReady === undefined) {
    setInterval(() => { window.prerenderReady = document.getElementsByTagName('vn-responsive-footer') !== undefined; }, 100);
}
");

            await page.WaitForFunctionAsync("() => window.prerenderReady === true");
        }
    }
}
