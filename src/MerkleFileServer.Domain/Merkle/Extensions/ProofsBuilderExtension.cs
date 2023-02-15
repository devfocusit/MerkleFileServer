namespace MerkleFileServer.Domain.Merkle.Extensions
{
    public static class ProofsBuilderExtension
    {
        public static IReadOnlyList<string> BuildProofs(this MerkleTree merkleTree, int leafIndex)
        {
            var leafNode = merkleTree.FindLeaf(leafIndex);

            var proofs = new List<MerkleNode>();

            AppendProof(leafNode, proofs);

            return proofs.Select(x => x.ToHashWithSide()).ToList();
        }

        private static void AppendProof(MerkleNode node, List<MerkleNode> proofNodes)
        {
            if (node is null)
            {
                return;
            }

            var sibling = node.GetSibling();

            if (sibling is not null)
            {
                proofNodes.Add(sibling);
            }

            AppendProof(node.ParentNode, proofNodes);
        }
    }
}
