using System.Collections.Generic;
using System.Xml.Serialization;

namespace Stormbus.UI.Containers
{
    public class ConnectionType : NotifyPropertyChanged
    {
        public static readonly string Serial = @"Serial Port";
        public static readonly string TcpIp = @"TCP/IP";
        public static readonly string UdpIp = @"UDP/IP";
        public static readonly string RtuAsciiOverTcpIp = @"RTU/ASCII over TCP/IP";
        public static readonly string RtuAsciiOverUdpIp = @"RTU/ASCII over UDP/IP";

        [XmlIgnore]
        public List<string> SupportedTypes { get; } = new List<string>
            {Serial, TcpIp, UdpIp, RtuAsciiOverTcpIp, RtuAsciiOverUdpIp};

        public string CurrentType { get; set; } = TcpIp;
    }
}