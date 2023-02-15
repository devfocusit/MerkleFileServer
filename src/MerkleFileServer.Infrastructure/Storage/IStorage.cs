using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkleFileServer.Infrastructure.Storage
{
    public interface IStorage
    {
        void Store<T>(string key, T content);
        object Read(string key);
    }
}
