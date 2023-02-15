using MerkleFileServer.Domain.Merkle;
using MerkleFileServer.Domain.Merkle.Extensions;
using System.Text;
using Xunit;

namespace MerkleFileServer.Tests
{
    public class MerkleTreeTests
    {
        public record TestData(List<string> pieces, string expectedRoodHash);

        private static TestData EvenNumberOfPieces = new TestData(
             new List<string>()
                {
                    "test1",
                    "test2",
                    "test3",
                    "test4"
                },
            "CCB46C7C1C7BFBFCA1E94748884AD15A9EEF697542515313B9A9ECB7E3131F3C");

        private static TestData OddNumberOfPieces = new TestData(
            new List<string>()
               {
                    "test1",
                    "test2",
                    "test3",
                    "test4",
                    "test5"
               },
           "1E315FD6554BF9D7C62934140ABAB41B44CECEB00C7B7951200EDC347F243272");

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] { EvenNumberOfPieces };
            yield return new object[] { OddNumberOfPieces };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void IsTreeValid(TestData testData)
        {
            //Arrange
            var pieces = testData.pieces.Select(c => Encoding.UTF8.GetBytes(c)).ToList();

            var merkleTree = new MerkleTree();

            pieces.ForEach(c => merkleTree.AddLeaf(c, pieces.IndexOf(c)));

            //Act
            merkleTree.Build();

            bool proofsAreValid = ValidateProofs(merkleTree, pieces);

            //Assert
            Assert.True(merkleTree.RootNode.IsValid());
            Assert.Equal(testData.expectedRoodHash, merkleTree.RootNode.HashHex);
            Assert.True(proofsAreValid);
        }

        private bool ValidateProofs(MerkleTree merkleTree, List<byte[]> content)
        {
            var validator = new MerkleProofsValidator(merkleTree.RootNode.Hash);

            bool isValid = true;
            for (int i = 0; i < merkleTree.NumberOfLeaves; i++)
            {
                var proofs = merkleTree.BuildProofs(i);
                isValid &= validator.Validate(content[i], proofs);
            }

            return isValid;
        }
    }
}