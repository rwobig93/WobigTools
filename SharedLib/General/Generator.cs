using System;
using System.Collections.Generic;

namespace SharedLib.General
{
    public static class Generator
    {
        // Default size of random password is 15  
        public static string CreateRandomPassword(int length = 15)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public static string CreateRandomPasswordWithRandomLength()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();
            
            // Minimum size 8. Max size is number of all allowed chars.  
            int size = random.Next(8, validChars.Length);

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public static int GetRandomNumberBetween(int startNumber, int endNumber)
        {
            Random random = new Random();
            return random.Next(startNumber, endNumber);
        }

        public static Dictionary<string, string> GetHttpHeadersToSend()
        {
            return new Dictionary<string, string>()
            {
                { "Connection", "keep-alive" },
                { "Cache-Control", "max-age=0" },
                { "DNT", "1" },
                { "Upgrade-Insecure-Requests", "1" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36" },
                { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                { "Sec-Fetch-Site", "none" },
                { "Sec-Fetch-Mode", "navigate" },
                { "Sec-Fetch-User", "?1" },
                { "Sec-Fetch-Dest", "document" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "Accept-Language", "en-US,en;q=0.9" }
            };
        }
    }
}
