using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using SharedLib.Dto;
using SharedLib.Extensions;

namespace CoreLogicLib.Comm
{
    public static class Web
    {
        public static HttpClient HttpClient { get; private set; }

        public static void Initialize()
        {
            if (HttpClient == null)
            {
                HttpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Log.Debug("Initialized HttpClient");
            }
            else
            {
                Log.Warning("HttpClient isn't null but we're attempting to initialize anyway");
            }
        }

        public static void Dispose()
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
            }
            else
            {
                Log.Warning("HttpClient was already null but we're attempting to dispose anyway");
            }
        }

        public static async Task<WebCheck> WebCheckForKeyword(string pageURL, string keyword)
        {
            if (HttpClient == null)
            {
                Initialize();
            }
            var uri = new Uri(pageURL);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddHttpHeaders(request);

            using var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            Log.Verbose("Webpage Contents:   {WebpageContents}", contents);
            if (string.IsNullOrWhiteSpace(contents))
            {
                return null;
            }
            else if (contents.Contains("The browser you're using doesn't support JavaScript, or has JavaScript turned off."))
            {
                throw new Exception("Javascript required, bot blocking method in place");
            }
            bool keywordFound = contents.Contains(keyword);
            bool pageCompressed = contents.IsCompressed();
            if (!pageCompressed)
            {
                return new WebCheck()
                {
                    KeywordExists = keywordFound,
                    ResponseCode = response.StatusCode,
                    WebpageContents = contents,
                    WasCompressed = pageCompressed
                };
            }
            Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (response.Content.Headers.ContentEncoding.ToString().ToLower().Contains("gzip"))
            {
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            }
            else if (response.Content.Headers.ContentEncoding.ToString().ToLower().Contains("deflate"))
            {
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
            }
            else
            {
                responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
            }
            using var streamReader = new StreamReader(responseStream);
            var stream = await streamReader.ReadToEndAsync();
            keywordFound = stream.Contains(keyword);
            Log.Verbose("Webpage Uncompressed: {WebpageUncompressed}", stream);
            return new WebCheck()
            {
                KeywordExists = keywordFound,
                ResponseCode = response.StatusCode,
                WebpageContents = contents,
                DecompressedContents = stream,
                WasCompressed = pageCompressed
            };
        }

        private static void AddHttpHeaders(HttpRequestMessage request)
        {
            Log.Verbose("Adding Http Headers");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-User", "?1");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            Log.Verbose("Finished Adding Http Headers");
        }

        internal static async Task<WebCheck> GetWebFileContentsUncompressed(string pageURL)
        {
            if (HttpClient == null)
            {
                Initialize();
            }
            var uri = new Uri(pageURL);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddHttpHeaders(request);

            using var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            Log.Verbose("Webpage Content: {WebpageCompressed}", contents);
            return new WebCheck()
            {
                ResponseCode = response.StatusCode,
                WebpageContents = contents
            };
        }
    }
}
