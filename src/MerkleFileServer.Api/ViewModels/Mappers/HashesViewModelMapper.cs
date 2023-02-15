using MerkleFileServer.Domain.Models;

namespace MerkleFileServer.Api.ViewModels.Mappers
{
    public static class HashesViewModelMapper
    {
        public static HashesViewModel ToViewModel(this HashedFile source)
        {
            return new HashesViewModel(source.RootHash, source.NumberOfPieces);
        }
    }
}
