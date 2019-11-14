using System.Linq;
using System.Net;
using SharpPcap;

namespace WC3Stats
{
    public static class IpAddressExtensions
    {
        public static ICaptureDevice GetDefaultCaptureDevice(this IPAddress ip)
        {
            var nicName = NIC.GetDefaultId(ip);
            var device = CaptureDeviceList.Instance
                .SingleOrDefault(x => x.Name.Replace(@"\Device\NPF_", "") == nicName);
            return device;
        }
    }
}