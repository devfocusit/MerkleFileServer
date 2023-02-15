using MerkleFileServer.Domain.Abstractions;

namespace MerkleFileServer.Infrastructure
{
    internal class FileLoader : IFileLoader
    {
        private const int DEAFULT_PIECE_SIZE_IN_KB = 1;

        public async Task<IReadOnlyList<Memory<byte>>> ToPieces(string fileFullName, int pieceSizeInKB = DEAFULT_PIECE_SIZE_IN_KB)
        {
            return await Task.Run(() =>
            {
                var pieces = new List<Memory<byte>>();

                var bufferSize = pieceSizeInKB * 1024;

                using (var file = File.Open(fileFullName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(file))
                    {
                        while (!IsEOF(reader))
                        {
                            /* If this is done in background thread then with each two pieces loaded a tree could be updated - paralleling hashing with reading and also limiting number of pieces hold in memory at same time
                             * Reading could be flusing into shared buffer listend by hash calculator
                             */
                            var content = new byte[bufferSize];
                            reader.Read(content, 0, content.Length);

                            pieces.Add(new Memory<byte>(content));
                        }

                        return pieces;
                    }
                }
            });
        }

        private bool IsEOF(BinaryReader reader)
        {
            var length = reader.BaseStream.Length;
            return reader.BaseStream.Position == length;
        }
    }
}