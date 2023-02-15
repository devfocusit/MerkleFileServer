using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Infrastructure.Storage;
using MerkleFileServer.Infrastructure.Storage.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MerkleFileServer.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IFilePieceRepository, FilePieceRepository>();
            services.AddSingleton<IMerkleTreeRepository, MerkleTreeRepository>();
            services.AddSingleton<IFileLoader, FileLoader>();
        }

        public static void ConfigureForMemoryStorage(this IServiceCollection services, Action<MemoryStorageOptions> options)
        {
            services.AddSingleton<IStorage, MemoryStorage>();
            var storageOptions = new MemoryStorageOptions();
            options.Invoke(storageOptions);

            services.AddSingleton(storageOptions);
        }
    }
}
