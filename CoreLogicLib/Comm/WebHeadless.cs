using PuppeteerSharp;
using Serilog;
using SharedLib.Dto;
using System.Threading.Tasks;

namespace CoreLogicLib.Comm
{
    public static class WebHeadless
    {
        public static Browser HeadlessBrowser { get; private set; }

        public static async Task Initialize(HeadlessBrowserType browserType = HeadlessBrowserType.Local)
        {
            if (HeadlessBrowser is null)
            {
                switch (browserType)
                {
                    case HeadlessBrowserType.Local:
                        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                        HeadlessBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
                        {
                            Headless = true,
                            Args = new string[]
                            {
                                "--disable-gpu",
                                "--disable-dev-shm-usage",
                                "--disable-setuid-sandbox",
                                "--no-first-run",
                                "--no-sandbox",
                                "--no-zygote",
                                "--deterministic-fetch",
                                "--disable-features=IsolateOrigins",
                                "--disable-site-isolation-trials"
                            }
                        });
                        break;
                    case HeadlessBrowserType.Remote:
                        HeadlessBrowser = await Puppeteer.ConnectAsync(new ConnectOptions()
                        {
                            BrowserWSEndpoint = "$wss://chrome.browserless.io"
                        });
                        break;
                }
            }
        }

        public static async Task Dispose()
        {
            if (HeadlessBrowser != null)
            {
                await HeadlessBrowser.CloseAsync();
            }
        }

        public static async Task<WebCheck> WebCheckForKeyword(string pageURL, string keyword, HeadlessBrowserType browserType = HeadlessBrowserType.Local)
        {
            if (HeadlessBrowser is null)
            {
                await Initialize(browserType);
            }

            var newPage = await HeadlessBrowser.NewPageAsync();
            await newPage.SetExtraHttpHeadersAsync(SharedLib.General.Generator.GetHttpHeadersToSend());
            var pageResponse = await newPage.GoToAsync(pageURL);
            var contents = await newPage.GetContentAsync();

            Log.Verbose("Webpage Contents:   {WebpageContents}", contents);
            if (string.IsNullOrWhiteSpace(contents))
            {
                return null;
            }
            bool keywordFound = contents.Contains(keyword);

            await newPage.DisposeAsync();

            return new WebCheck()
            {
                KeywordExists = keywordFound,
                ResponseCode = pageResponse.Status,
                WebpageContents = contents,
                DecompressedContents = "N/A",
                WasCompressed = false
            };
        }
    }
}