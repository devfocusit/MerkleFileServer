using MerkleFileServer.Addon.AsClient.ApiClient;
using MerkleFileServer.Addon.AsClient.ApiClient.Contract;
using MerkleFileServer.Addon.AsClient.PeerWorker;
using Microsoft.Extensions.DependencyInjection;

namespace MerkleFileServer.Addon.AsClient
{
    public static class ServiceCollectionExtensions
    {
        public static void AddClientCapabilities(this IServiceCollection services, Action<ClientOptions> options)
        {
            var clientOptions = new ClientOptions();
            options.Invoke(clientOptions);

            services.AddSingleton(clientOptions);

            services.AddHttpClient();

            services.AddScoped<IClientSimulationService, ClientSimulationService>();
            services.AddScoped<IMerkleFileServerApiClient, MerkleFileServerApiClient>();
            services.AddScoped<IPeerWorkerFactory, PeerWorkerFactory>();
        }
    }
}
