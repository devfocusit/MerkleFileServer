using System.Security.Cryptography;

namespace MerkleFileServer.Domain.Merkle
{
    public static class HashCalculator
    {
        public static byte[] ConcatenateTwoHashes(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            if (left.Length != 32)
            {
                throw new ArgumentException("Hash must be 32 bytes long", nameof(left));
            }

            if (right != null && right.Length != 32)
            {
                throw new ArgumentException("Hash must be 32 bytes long", nameof(right));
            }

            Span<byte> pair = stackalloc byte[64];

            left.CopyTo(pair);
            right.CopyTo(pair[32..]);

            var hash = ComputeHash(pair);

            return hash;
        }

        public static byte[] ComputeHash(ReadOnlySpan<byte> input)
        {
            Span<byte> result = stackalloc byte[32];
            SHA256.HashData(input, result);
            return SHA256.HashData(result);
        }
    }
}
