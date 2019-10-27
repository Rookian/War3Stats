using PacketDotNet;

namespace WC3Stats
{
    public static class TcpPacketExtensions
    {
        public static bool IsInIpRange(this TcpPacket tcpPacket)
        {
            var ipPacket = (IPPacket)tcpPacket.ParentPacket;
            var source = ipPacket.SourceAddress.ToString();
            var destination = ipPacket.DestinationAddress.ToString();

            return destination.StartsWith("5.42.181") || source.StartsWith("5.42.181");
        }
    }
}