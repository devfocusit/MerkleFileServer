using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Addon.AsClient.ApiClient
{
    public class ClientOptions
    {
        public string TrustedServer { get; set; }
        public IReadOnlyList<string> Peers { get; set; }
    }
}
