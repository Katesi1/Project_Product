using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MvcLaptop.Utils
{
    public static class UtilityHelper
    {
        /// <summary>
        /// Generates a HMAC SHA-512 hash for the given input data using the specified key.
        /// </summary>
        /// <param name="key">The key to use for the HMAC algorithm.</param>
        /// <param name="inputData">The data to hash.</param>
        /// <returns>The HMAC SHA-512 hash as a hexadecimal string.</returns>
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            Console.WriteLine("Key: " + key);   
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            Console.WriteLine("Input: " + inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        /// <summary>
        /// Retrieves the IP address of the client from the given HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>The IP address as a string.</returns>
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return "Invalid IP:" + ex.Message;
            }

            return "127.0.0.1";
        }

        public static string? GenerateFileNameToSave(string incomingFileName)
        {
            // var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{extension}";
        }

        public static string? GetRolesInClaim(string? claimJson)
        {
            if (string.IsNullOrEmpty(claimJson)) return null;
            var roles = JsonConvert.DeserializeObject<string[]>(claimJson);
            if (roles == null) return null;
            return string.Join(", ", roles);
        }
    }
}
