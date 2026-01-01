using System;

namespace DotNetPlugin
{
    /// <summary>
    /// Configuration for the MCP server including IP address and port settings.
    /// Settings are stored in-memory only and reset when x64dbg restarts.
    /// </summary>
    public class McpServerConfig
    {
        public string IpAddress { get; set; } = "+";
        public int Port { get; set; } = 50300;

        /// <summary>
        /// Gets the base URL for the HTTP listener (e.g., "http://+:50300/").
        /// </summary>
        public string GetBaseUrl()
        {
            return $"http://{IpAddress}:{Port}/";
        }

        /// <summary>
        /// Gets the SSE endpoint URL for display purposes.
        /// </summary>
        public string GetDisplayUrl()
        {
            string displayIp = (IpAddress == "+" || IpAddress == "*") ? "127.0.0.1" : IpAddress;
            return $"http://{displayIp}:{Port}/sse";
        }

        /// <summary>
        /// Validates the IP address format.
        /// </summary>
        public static bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            // Special values for HttpListener
            if (ip == "+" || ip == "*" || ip == "localhost")
                return true;

            // Check for valid IP address format
            System.Net.IPAddress address;
            return System.Net.IPAddress.TryParse(ip, out address);
        }

        /// <summary>
        /// Validates the port number.
        /// </summary>
        public static bool IsValidPort(int port)
        {
            return port >= 1 && port <= 65535;
        }
    }
}
