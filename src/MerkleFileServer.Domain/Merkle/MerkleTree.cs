namespace MerkleFileServer.Domain.Merkle
{
    public class MerkleTree
    {
        private IList<MerkleNode> _leaves;

        public MerkleNode RootNode { get; private set; }

        public MerkleTree()
        {
            _leaves = new List<MerkleNode>();
        }

        public void AddLeaf(ReadOnlySpan<byte> content, int index)
        {
            _leaves.Add(MerkleNode.CreateLeafNode(content, index));
        }

        public void Build()
        {
            if (!_leaves.Any())
            {
                return;
            }

            RootNode = Build(_leaves);
        }

        public int NumberOfLeaves { get { return _leaves.Count; } }

        public MerkleNode? FindLeaf(int leafIndex)
        {
            return _leaves.FirstOrDefault(x => x.LeafIndex == leafIndex);
        }

        private MerkleNode Build(IList<MerkleNode> nodes)
        {
            if (!nodes.Any())
            {
                throw new Exception("Unexpected data");
            }

            if (nodes.Count == 1)
            {
                return nodes.First();
            }

            var branchNodes = new List<MerkleNode>();

            for (int i = 0; i < nodes.Count; i += 2)
            {
                var left = nodes[i];
                var right = i + 1 < nodes.Count ? nodes[i + 1] : null;

                var branchNode = MerkleNode.CreateBranchNode(left, right);

                branchNodes.Add(branchNode);
            }

            return Build(branchNodes);
        }
    }
}
