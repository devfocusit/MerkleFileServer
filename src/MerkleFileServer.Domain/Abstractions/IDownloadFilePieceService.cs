using MerkleFileServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Domain.Abstractions
{
    public interface IDownloadFilePieceService
    {
        FilePiece? TryDownload(string rootHash, int pieceIndex);
    }
}
