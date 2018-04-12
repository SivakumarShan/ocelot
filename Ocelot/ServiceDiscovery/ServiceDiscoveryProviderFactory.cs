using System.Collections.Generic;
using Ocelot.Configuration;
using Ocelot.Logging;
using Ocelot.Values;

namespace Ocelot.ServiceDiscovery
{
    public class ServiceDiscoveryProviderFactory : IServiceDiscoveryProviderFactory
    {
        private readonly IOcelotLoggerFactory _factory;

        public ServiceDiscoveryProviderFactory(IOcelotLoggerFactory factory)
        {
            _factory = factory;
        }

        public IServiceDiscoveryProvider Get(ServiceProviderConfiguration serviceConfig, DownstreamReRoute reRoute)
        {
            if (reRoute.UseServiceDiscovery)
            {
                return GetServiceDiscoveryProvider(serviceConfig, reRoute.ServiceName);
            }

            var services = new List<Service>();

            foreach (var downstreamAddress in reRoute.DownstreamAddresses)
            {
                var service = new Service(reRoute.ServiceName, new ServiceHostAndPort(downstreamAddress.Host, downstreamAddress.Port), string.Empty, string.Empty, new string[0]);
                
                services.Add(service);
            }

            return new ConfigurationServiceProvider(services);
        }

        private IServiceDiscoveryProvider GetServiceDiscoveryProvider(ServiceProviderConfiguration serviceConfig, string serviceName)
        {
            if (serviceConfig.Type == "ServiceFabric")
            {
                var config = new ServiceFabricConfiguration(serviceConfig.Host, serviceConfig.Port, serviceName);
                return new ServiceFabricServiceDiscoveryProvider(config);
            }

            //if (serviceConfig.Type == "Steeltoe")
            //{
            //    var steeltoeConfig = new Steeltoe.SteeltoeRegistryConfiguration(serviceConfig.Host, serviceConfig.Port, serviceName);
            //    return new Steeltoe.SteeltoeServiceDiscoveryProvider(steeltoeConfig, _factory);
            //}

            var consulRegistryConfiguration = new ConsulRegistryConfiguration(serviceConfig.Host, serviceConfig.Port, serviceName);
            return new ConsulServiceDiscoveryProvider(consulRegistryConfiguration, _factory);
        }
    }
}
