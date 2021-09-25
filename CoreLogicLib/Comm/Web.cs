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
            foreach (var header in SharedLib.General.Generator.GetHttpHeadersToSend())
            {
                request.Headers.Add(header.Key, header.Value);
            }
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
