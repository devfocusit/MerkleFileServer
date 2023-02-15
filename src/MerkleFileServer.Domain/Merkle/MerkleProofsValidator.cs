using MerkleFileServer.Domain.Merkle.Extensions;

namespace MerkleFileServer.Domain.Merkle
{
    public class MerkleProofsValidator
    {
        private readonly byte[] _rootHash;

        public MerkleProofsValidator(string rootHashHex)
            : this(Convert.FromHexString(rootHashHex))
        {
        }

        public MerkleProofsValidator(byte[] rootHash)
        {
            _rootHash = rootHash ?? throw new ArgumentNullException(nameof(rootHash));
        }

        public bool Validate(ReadOnlySpan<byte> content, IReadOnlyList<string> proofs)
        {
            var leafHash = HashCalculator.ComputeHash(content);

            var pairHash = Compute(proofs[0], leafHash);

            for (int i = 1; i < proofs.Count; i++)
            {
                pairHash = Compute(proofs[i], pairHash);
            }

            return ((ReadOnlySpan<byte>)_rootHash).SequenceEqual(pairHash);
        }

        private byte[] Compute(string proof, ReadOnlySpan<byte> siblingHash)
        {
            var proofWithSide = proof.ToProofWithSide();

            var proofHash = Convert.FromHexString(proofWithSide.Item1);

            if (proofWithSide.Item2 == NodeSide.Left)
            {
                return HashCalculator.ConcatenateTwoHashes(proofHash, siblingHash);
            }
            else
            {
                return HashCalculator.ConcatenateTwoHashes(siblingHash, proofHash);
            }
        }
    }
}
