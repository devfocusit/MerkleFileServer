using MerkleFileServer.Api.ViewModels;
using MerkleFileServer.Api.ViewModels.Mappers;
using MerkleFileServer.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using MerkleFileServer.Api.ViewModels.Mappers;

namespace MerkleFileServer.Api.Controllers
{
    [ApiController()]
    [Route("piece")]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadFilePieceService _downloadFilePieceService;

        public DownloadController(IDownloadFilePieceService downloadFilePieceService)
        {
            _downloadFilePieceService = downloadFilePieceService ?? throw new ArgumentNullException(nameof(downloadFilePieceService));
        }

        /// <summary>
        /// Returns a verifiable piece of the content
        /// </summary>
        /// <param name="hashId">The merkle hash of the file we want to download</param>
        /// <param name="pieceIndex">The index of the piece we want to download</param>
        /// <returns></returns>
        [HttpGet("{hashId}/{pieceIndex}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<FilePieceViewModel>), 200)]
        public IActionResult GetPiece([FromRoute] string hashId, [FromRoute] int pieceIndex)
        {
            var result = _downloadFilePieceService.TryDownload(hashId, pieceIndex);

            if (result is null)
            {
                return NotFound();
            }

            return new OkObjectResult(result.ToViewModel());
        }
    }
}