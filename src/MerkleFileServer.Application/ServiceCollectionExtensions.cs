using MerkleFileServer.Application.DownloadFilePiece;
using MerkleFileServer.Application.GetAvailableHashes;
using MerkleFileServer.Application.LoadFileHash;
using MerkleFileServer.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MerkleFileServer.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILoadFileHashService, LoadFileHashService>();
            services.AddScoped<IGetAvailableHashesService, GetAvailableHashesService>();
            services.AddScoped<IDownloadFilePieceService, DownloadFilePieceService>();
        }
    }
}
