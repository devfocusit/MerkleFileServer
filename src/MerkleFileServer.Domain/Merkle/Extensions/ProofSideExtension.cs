namespace MerkleFileServer.Domain.Merkle.Extensions
{
    /* Adding side-orientation char to proof makes it easier for later computing at validation
     * Also this limits probability of second preimage attack when certain hashes are expected on specific position
     */

    public static class ProofSideExtension
    {
        private const char LEFT = 'L';
        private const char RIGHT = 'R';

        public static string ToHashWithSide(this MerkleNode node)
        {
            var sideChar = node.GetSide() == NodeSide.Left ? LEFT : RIGHT;
            return sideChar + node.HashHex;
        }

        public static Tuple<string, NodeSide> ToProofWithSide(this string proof)
        {
            var side = proof.ResolveProofSide();

            var proofHash = proof.Substring(1);

            return new Tuple<string, NodeSide>(proofHash, side);
        }

        private static NodeSide ResolveProofSide(this string proof)
        {
            if (proof.StartsWith(LEFT))
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
