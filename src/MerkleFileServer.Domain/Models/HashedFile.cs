using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Domain.Models
{
    public record HashedFile(FileId FileId, string RootHash, int NumberOfPieces);
}
