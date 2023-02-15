using MerkleFileServer.Domain.Merkle;
using System.Text;
using Xunit;

namespace MerkleFileServer.Tests
{
    public class MerkleNodeTests
    {
        [Fact]
        public void IsNodeSiblingValid()
        {
            //Arrange
            var content1 = Encoding.UTF8.GetBytes("test1");
            var content2 = Encoding.UTF8.GetBytes("test2");

            var leafNode1 = MerkleNode.CreateLeafNode(content1, 0);
            var leafNode2 = MerkleNode.CreateLeafNode(content2, 1);

            var branchNode = MerkleNode.CreateBranchNode(leafNode1, leafNode2);

            //Act & Assert
            Assert.True(branchNode.IsValid());
            Assert.True(leafNode2.GetSibling() == leafNode1);
            Assert.True(leafNode1.GetSibling() == leafNode2);
            Assert.Null(branchNode.GetSibling());
            Assert.True(leafNode1.GetSide() == NodeSide.Left);
            Assert.True(leafNode2.GetSide() == NodeSide.Right);
        }

        [Fact]
        public void IsNodeValidWhenRightNodeIsNull()
        {
            //Arrange
            var content1 = Encoding.UTF8.GetBytes("test1");

            var leafNode1 = MerkleNode.CreateLeafNode(content1, 0);

            var branchNode = MerkleNode.CreateBranchNode(leafNode1, null);

            //Act
            var sibling = leafNode1.GetSibling();

            //Assert
            Assert.True(branchNode.IsValid());
            Assert.NotNull(sibling);
            Assert.True(sibling?.NodeType == NodeType.Filler);
            Assert.True(leafNode1.GetSide() == NodeSide.Left);
        }
    }
}