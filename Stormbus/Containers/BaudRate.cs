using System.Collections.Generic;
using System.Xml.Serialization;

namespace Stormbus.UI.Containers
{
    public class BaudRate : NotifyPropertyChanged
    {
        [XmlIgnore]
        public List<int> SupportedBaudRates { get; } = new List<int>
        {
            300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 230400, 256000
        };

        public int Current { get; set; } = 9600;
    }
}