using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Addon.AsClient.ApiClient.Contract
{
    public record FilePieceViewModel(string fileName, Memory<byte> content, IReadOnlyList<string> proofs);
}
