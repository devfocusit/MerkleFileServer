using MerkleFileServer.Api.ViewModels.Mappers;
using MerkleFileServer.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MerkleFileServer.Api.Controllers
{
    [ApiController()]
    [Route("hashes")]
    public class TrustedServerController : ControllerBase
    {
        private readonly IGetAvailableHashesService _getHashesService;
        private readonly ILoadFileHashService _loadFileHashService;

        public TrustedServerController(IGetAvailableHashesService getHashesService,
            ILoadFileHashService loadFileHashService)
        {
            _getHashesService = getHashesService ?? throw new ArgumentNullException(nameof(getHashesService));
            _loadFileHashService = loadFileHashService ?? throw new ArgumentNullException(nameof(loadFileHashService));
        }

        /// <summary>
        /// Returns a json list of the merkle hashes and number of pieces of the files this server is serving
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult GetAvailableHashes()
        {
            var availableHashes = _getHashesService.GetHashes();

            return new OkObjectResult(availableHashes.Select(x => x.ToViewModel()));
        }

        /// <summary>
        /// Loads new file into server as hashed pieces
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{file}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Load([FromRoute] string file)
        {
            await _loadFileHashService.Load(file);

            return Accepted();
        }
    }
}