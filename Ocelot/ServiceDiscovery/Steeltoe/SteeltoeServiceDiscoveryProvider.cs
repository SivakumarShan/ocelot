using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ocelot.Logging;
using Ocelot.Values;

namespace Ocelot.ServiceDiscovery.Steeltoe
{
    public class SteeltoeServiceDiscoveryProvider : IServiceDiscoveryProvider
    {
        private readonly IOcelotLogger _logger;
        private readonly SteeltoeRegistryConfiguration _steeltoeConfig;
        public SteeltoeServiceDiscoveryProvider(SteeltoeRegistryConfiguration configuration, IOcelotLoggerFactory factory)
        {
            _logger = factory.CreateLogger<SteeltoeServiceDiscoveryProvider>();

            var steeltoeHost = string.IsNullOrEmpty(configuration?.HostName) ? "localhost" : configuration.HostName;

            var steeltoePort = configuration?.Port ?? 8500;

            _steeltoeConfig = new SteeltoeRegistryConfiguration(steeltoeHost, steeltoePort, configuration?.ServiceName);
        }

        public async Task<List<Service>> Get()
        {
            return new List<Service>
            {
                new Service(_steeltoeConfig.ServiceName,
                    new ServiceHostAndPort(_steeltoeConfig.HostName, _steeltoeConfig.Port),
                    "doesnt matter with service fabric",
                    "doesnt matter with service fabric",
                    new List<string>())
            };
        }
    }
}
