using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Domain.Abstractions
{
    public interface IFileLoader
    {
        Task<IReadOnlyList<Memory<byte>>> ToPieces(string fileFullName, int pieceSizeInKB);
    }
}