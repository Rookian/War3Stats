using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace WC3Stats
{
    public static class NIC
    {
        [DllImport("iphlpapi.dll", CharSet = CharSet.Auto)]
        private static extern int GetBestInterface(uint destAddr, out uint bestIfIndex);

        public static NetworkInterface GetDefaultNetworkInterface(IPAddress destinationAddress)
        {
            var destaddr = BitConverter.ToUInt32(destinationAddress.GetAddressBytes(), 0);

            int result = GetBestInterface(destaddr, out var interfaceIndex);
            if (result != 0)
                throw new Win32Exception(result);

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces().OrderBy(x => x.Description))
            {
                var ipProperties = networkInterface.GetIPProperties();

                var gateway = ipProperties?.GatewayAddresses?.FirstOrDefault()?.Address;
                if (gateway == null)
                    continue;

                if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
                {
                    var iPv4Properties = ipProperties.GetIPv4Properties();
                    if (iPv4Properties == null)
                        continue;

                    if (iPv4Properties.Index == interfaceIndex)
                        return networkInterface;
                }

                if (networkInterface.Supports(NetworkInterfaceComponent.IPv6))
                {
                    var iPv6Properties = ipProperties.GetIPv6Properties();
                    if (iPv6Properties == null)
                        continue;

                    if (iPv6Properties.Index == interfaceIndex)
                        return networkInterface;
                }
            }

            return null;
        }
    }
}