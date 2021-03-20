﻿using System.Net;

namespace SharedLib.Dto
{
    public class WebCheck
    {
        public bool KeywordExists { get; set; }
        public bool WasCompressed { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public string WebpageContents { get; set; }
        public string DecompressedContents { get; set; }
    }
}
