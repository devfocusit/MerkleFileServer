using MerkleFileServer.Domain.Merkle;
using System.Text;
using Xunit;

namespace MerkleFileServer.Tests
{
    public class HashCalculatorTests
    {
        [Fact]
        public void HashesForSameContentAreEqual()
        {
            //Arrange

            //Act
            var hash1 = HashCalculator.ComputeHash(Encoding.UTF8.GetBytes("test"));
            var hash2 = HashCalculator.ComputeHash(Encoding.UTF8.GetBytes("test"));

            //Assert
            Assert.True(((ReadOnlySpan<byte>)hash1).SequenceEqual(hash2));
        }

        [Fact]
        public void HashForNullRightNodeIsValid()
        {
            //Arrange
            var hash1 = HashCalculator.ComputeHash(Encoding.UTF8.GetBytes("1st test"));

            Span<byte> pair = stackalloc byte[64];
            hash1.CopyTo(pair);

            var expectedHash = HashCalculator.ComputeHash(pair);

            //Act
            var hash = HashCalculator.ConcatenateTwoHashes(hash1, null);

            //Assert
            Assert.True(((ReadOnlySpan<byte>)expectedHash).SequenceEqual(hash));
        }

        [Fact]
        public void HashForTwoNodesIsValid()
        {
            //Arrange
            var hash1 = HashCalculator.ComputeHash(Encoding.UTF8.GetBytes("1st test"));
            var hash2 = HashCalculator.ComputeHash(Encoding.UTF8.GetBytes("2nd test"));

            Span<byte> pair = stackalloc byte[64];
            hash1.CopyTo(pair);
            hash2.CopyTo(pair[32..]);

            var expectedHash = HashCalculator.ComputeHash(pair);

            //Act
            var hash = HashCalculator.ConcatenateTwoHashes(hash1, hash2);

            //Assert
            Assert.True(((ReadOnlySpan<byte>)expectedHash).SequenceEqual(hash));
        }
    }
}