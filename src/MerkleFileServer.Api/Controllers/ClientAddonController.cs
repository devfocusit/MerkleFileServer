using MerkleFileServer.Addon.AsClient;
using Microsoft.AspNetCore.Mvc;

namespace MerkleFileServer.Api.Controllers
{
    [ApiController]
    [Route("asclient")]
    public class ClientAddonController : ControllerBase
    {
        private readonly IClientSimulationService _clientSimulationService;

        public ClientAddonController(IClientSimulationService clientSimulationService)
        {
            _clientSimulationService = clientSimulationService ?? throw new ArgumentNullException(nameof(clientSimulationService));
        }

        /// <summary>
        /// Allows to act like a client downloading desired hashId. 
        /// If multiple peers are connected (defined in appsettings) then pieces are downloaded from multiple sources and merged into final file after validating proofs
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="destinationFile"></param>
        /// <param name="peers"></param>
        /// <returns></returns>
        [HttpGet("{hashId}/{destinationFile}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClientDownloadResponse), 200)]
        public async Task<IActionResult> Download(
            [FromRoute] string hashId,
            [FromRoute] string destinationFile)
        {
            var result = await _clientSimulationService.Download(hashId, destinationFile);

            return new OkObjectResult(result);
        }

        /// <summary>
        /// Same as above - just working always for first available hashId and storing result content into fixed file
        /// </summary>
        /// <param name="peers"></param>
        /// <returns></returns>
        [HttpGet()]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClientDownloadResponse), 200)]
        public async Task<IActionResult> Download()
        {
            var result = await _clientSimulationService.Download(null, "destinationFile.png");

            return new OkObjectResult(result);
        }
    }
}