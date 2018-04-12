using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.ServiceDiscovery.Steeltoe
{
    public class SteeltoeRegistryConfiguration
    {
        public SteeltoeRegistryConfiguration(string hostName, int port, string serviceName)
        {
            HostName = hostName;
            Port = port;
            ServiceName = serviceName;
        }

        public string HostName { get; private set; }
        public int Port { get; private set; }
        public string ServiceName { get; private set; }
    }
}
