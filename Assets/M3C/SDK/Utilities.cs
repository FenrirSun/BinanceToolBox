using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace M3C.Finance.BinanceSdk
{
    public static class Utilities
    {
        public static string GetHexString(byte[] bytes) {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) {
                builder.Append($"{b:x2}");
            }

            return builder.ToString();
        }

        public static byte[] Sign(string secret, string content) {
            var signedBytes = new HMACSHA256(Encoding.UTF8.GetBytes(secret))
                .ComputeHash(Encoding.UTF8.GetBytes(content));
            return signedBytes;
        }

        private static readonly long EpocTicks = new DateTime(1970, 1, 1).Ticks;

        private static long TimeLocal
        {
            get { return (DateTime.UtcNow.Ticks - EpocTicks) / TimeSpan.TicksPerMillisecond; }
        }
        private static long serverTimeOffset;
        
        public static long GetCurrentMilliseconds() {
            var unixTime = TimeLocal - serverTimeOffset;
            return unixTime;
        }

        public static void OnGetServerTime(long serverTime) {
            serverTimeOffset = TimeLocal - serverTime;
        }
        
        public static DateTime DateFromUnixMs(long unixTime) {
            return new DateTime(unixTime * TimeSpan.TicksPerMillisecond + EpocTicks);
        }
    }
}