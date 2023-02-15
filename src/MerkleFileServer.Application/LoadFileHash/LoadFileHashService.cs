using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Domain.Merkle;
using MerkleFileServer.Domain.Merkle.Extensions;
using MerkleFileServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerkleFileServer.Application.LoadFileHash
{
    internal class LoadFileHashService : ILoadFileHashService
    {
        private readonly IFileLoader _fileLoader;
        private readonly IFilePieceRepository _filePieceStorage;
        private readonly IMerkleTreeRepository _merkleTreeMemoryStorage;

        public LoadFileHashService(
            IFileLoader fileLoader,
            IFilePieceRepository filePieceStorage,
            IMerkleTreeRepository merkleTreeMemoryStorage
            )
        {
            _merkleTreeMemoryStorage = merkleTreeMemoryStorage ?? throw new ArgumentNullException(nameof(merkleTreeMemoryStorage));
            _fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));
            _filePieceStorage = filePieceStorage ?? throw new ArgumentNullException(nameof(filePieceStorage));
        }

        public async Task Load(string fileFullName)
        {
            /* Instead reading all pieces before passing them for hashing it could be done with read-ahead / write-behind approach - with each two pieces read into memory. 
             * That would be a must for big files. 
             * Although due to that file's pieces need to use its full name as id for cache,  because root hash is not known, yet
             */
            var pieces = await _fileLoader.ToPieces(fileFullName, 1);

            var savePiecesTask = StartSavePieces(fileFullName, pieces);

            var merkleTree = BuildTree(pieces);

            RegisterNewFileLoaded(fileFullName, merkleTree.RootNode.HashHex, pieces.Count);

            RegisterProofs(fileFullName, merkleTree);

            await savePiecesTask;
        }

        private MerkleTree BuildTree(IReadOnlyList<Memory<byte>> pieces)
        {
            var merkleTree = new MerkleTree();

            for (int i = 0; i < pieces.Count; i++)
            {
                merkleTree.AddLeaf(pieces[i].Span, i);
            }

            merkleTree.Build();

            return merkleTree;
        }

        private Task StartSavePieces(string fileFullName, IReadOnlyList<Memory<byte>> pieces)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < pieces.Count; i++)
                {
                    _filePieceStorage.Save(fileFullName, i, pieces[i]);
                }
            });
        }

        private void RegisterNewFileLoaded(string fileFullName, string rootHash, int numberOfPieces)
        {
            var hashedFile = new HashedFile(new FileId(fileFullName), rootHash, numberOfPieces);
            _merkleTreeMemoryStorage.AddNewFile(hashedFile);
        }

        private void RegisterProofs(string fileFullName, MerkleTree merkleTree)
        {
            for (int i = 0; i < merkleTree.NumberOfLeaves; i++)
            {
                var proofs = merkleTree.BuildProofs(i);

                /* Proofs are stored separately, not with file piece itself, as it could be beneficial not to tight it togather 
                 * Otherwise in case of bigger files all content needs to be read into and hashed before calculating proofs. 
                 * This way it could use read-ahead approach.
                 */
                _merkleTreeMemoryStorage.AddProof(
                    merkleTree.RootNode.HashHex,
                    new LeafProof(new FileId(fileFullName), i, proofs));
            }
        }
    }
}
