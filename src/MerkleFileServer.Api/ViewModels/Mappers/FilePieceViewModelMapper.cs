using MerkleFileServer.Domain.Models;

namespace MerkleFileServer.Api.ViewModels.Mappers
{
    public static class FilePieceViewModelMapper
    {
        public static FilePieceViewModel ToViewModel(this FilePiece source)
        {
            return new FilePieceViewModel(source.fileId.FileName, source.Content, source.Proofs);
        }
    }
}
