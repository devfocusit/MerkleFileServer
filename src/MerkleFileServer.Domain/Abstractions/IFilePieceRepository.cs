namespace MerkleFileServer.Domain.Abstractions
{
    public interface IFilePieceRepository
    {
        void Save(string fileId, int pieceIndex, Memory<byte> content);
        Memory<byte> Fetch(string fileId, int pieceIndex);
    }

}
