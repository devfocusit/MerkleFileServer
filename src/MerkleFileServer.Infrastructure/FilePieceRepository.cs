using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Infrastructure.Storage;
using System;

namespace MerkleFileServer.Infrastructure
{
    internal class FilePieceRepository : IFilePieceRepository
    {
        private readonly IStorage _storage;

        public FilePieceRepository(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void Save(string fileId, int pieceIndex, Memory<byte> content)
        {
            var key = ToStorageKey(fileId, pieceIndex);

            _storage.Store(key, content);
        }

        public Memory<byte> Fetch(string fileId, int pieceIndex)
        {
            var key = ToStorageKey(fileId, pieceIndex);

            var item = _storage.Read(key);

            if (item is null)
            {
                throw new KeyNotFoundException();
            }

            return (Memory<byte>)item;
        }

        private string ToStorageKey(string fileId, int pieceIndex)
        {
            return $"{fileId}:{pieceIndex}";
        }
    }
}
