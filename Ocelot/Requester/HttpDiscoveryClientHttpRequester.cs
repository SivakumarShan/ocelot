using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responses;
using Pivotal.Discovery.Client;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Ocelot.Requester
{
    public class HttpDiscoveryClientHttpRequester : IHttpRequester
    {
        private readonly IHttpClientCache _cacheHandlers;
        private readonly IOcelotLogger _logger;
        private readonly IDelegatingHandlerHandlerHouse _house;
        private readonly IDiscoveryClient _discoveryClient;
        private DiscoveryHttpClientHandler _handler;

        public HttpDiscoveryClientHttpRequester(IOcelotLoggerFactory loggerFactory,
            IHttpClientCache cacheHandlers,
            IDelegatingHandlerHandlerHouse house, IDiscoveryClient discoveryClient)
        {
            _logger = loggerFactory.CreateLogger<HttpClientHttpRequester>();
            _cacheHandlers = cacheHandlers;
            _house = house;
            _discoveryClient = discoveryClient;
        }

        public async Task<Response<HttpResponseMessage>> GetResponseAsync(DownstreamContext request)
        {
            _handler = new DiscoveryHttpClientHandler(_discoveryClient);
            var discoveryClientBuilder = new DiscoveryHttpClientBuilder().Create(_handler, request.DownstreamReRoute);
            var httpDiscoveryClient = discoveryClientBuilder.Client;

            try
            {
                var response = await httpDiscoveryClient.SendAsync(request.DownstreamRequest);
                return new OkResponse<HttpResponseMessage>(response);
            }
            catch (TimeoutRejectedException exception)
            {
                return
                    new ErrorResponse<HttpResponseMessage>(new RequestTimedOutError(exception));
            }
            catch (BrokenCircuitException exception)
            {
                return
                    new ErrorResponse<HttpResponseMessage>(new RequestTimedOutError(exception));
            }
            catch (Exception exception)
            {
                return new ErrorResponse<HttpResponseMessage>(new UnableToCompleteRequestError(exception));
            }
            finally
            {
                //_cacheHandlers.Set(cacheKey, httpClient, TimeSpan.FromHours(24));
            }
        }
    }
}
