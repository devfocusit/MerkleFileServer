namespace MerkleFileServer.Api.ViewModels
{
    public record FilePieceViewModel(string FileName, Memory<byte> Content, IReadOnlyList<string> Proofs);
}
