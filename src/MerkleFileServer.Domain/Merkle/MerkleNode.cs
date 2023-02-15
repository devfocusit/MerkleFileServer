namespace MerkleFileServer.Domain.Merkle
{
    public enum NodeType { Leaf, Branch, Filler }

    public enum NodeSide { Left, Right, Root }

    public class MerkleNode
    {
        private static byte[] EMPTY_HASH = new byte[32];

        public NodeType NodeType { get; private set; }
        public int? LeafIndex { get; private set; }
        public byte[] Hash { get; private set; }
        public string HashHex { get; private set; }
        public MerkleNode LeftNode { get; private set; }
        public MerkleNode RightNode { get; private set; }
        public MerkleNode ParentNode { get; private set; }

        private MerkleNode(byte[] hash, MerkleNode left, MerkleNode? right, NodeType nodeType, int? leafIndex)
        {
            Hash = hash;
            HashHex = Convert.ToHexString(Hash);

            NodeType = nodeType;
            LeafIndex = leafIndex;
            LeftNode = left;
            RightNode = right;
        }

        public static MerkleNode CreateLeafNode(ReadOnlySpan<byte> content, int index)
        {
            var leafHash = HashCalculator.ComputeHash(content);

            return new MerkleNode(leafHash, null, null, NodeType.Leaf, index);
        }
        public static MerkleNode CreateFillerNode()
        {
            return new MerkleNode(EMPTY_HASH, null, null, NodeType.Filler, null);
        }

        public static MerkleNode CreateBranchNode(MerkleNode left, MerkleNode? right)
        {
            var pairHash = HashCalculator.ConcatenateTwoHashes(left.Hash, right?.Hash);

            var branchNode = new MerkleNode(pairHash, left, right, NodeType.Branch, null);

            left.ParentNode = branchNode;
            if (right is not null)
            {
                right.ParentNode = branchNode;
            }

            return branchNode;
        }

        public bool IsValid()
        {
            if (NodeType != NodeType.Branch)
            {
                return true;
            }

            var pairHash = HashCalculator.ConcatenateTwoHashes(LeftNode.Hash, RightNode?.Hash);

            return ((ReadOnlySpan<byte>)Hash).SequenceEqual(pairHash);
        }

        public MerkleNode? GetSibling()
        {
            if (ParentNode is null)
            {
                return null;
            }

            if (this == ParentNode.LeftNode)
            {
                return ParentNode.RightNode ?? CreateFillerNode();
            }
            else
            {
                return ParentNode.LeftNode;
            }
        }

        public NodeSide GetSide()
        {
            if (ParentNode is null)
            {
                return NodeSide.Root;
            }

            if (this == ParentNode.LeftNode)
            {
                return NodeSide.Left;
            }
            else
            {
                return NodeSide.Right;
            }
        }
    }
}
