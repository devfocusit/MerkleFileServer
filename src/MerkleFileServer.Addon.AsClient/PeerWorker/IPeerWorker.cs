using System.Collections.Concurrent;

namespace MerkleFileServer.Addon.AsClient.PeerWorker
{
    internal interface IPeerWorker
    {
        Task<PeerWorkerResult> Start(string rootHash, string peer, ConcurrentQueue<int> queue);
    }
}
