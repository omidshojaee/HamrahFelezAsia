using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HamrahFelez.Services;
using HamrahFelez.ViewModels;
using HamrahFelez.Utilities;

namespace HamrahFelez.WebAPI
{
    [Route("api/communication")]
    [ApiController]
    public class CommunicationController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;

        public CommunicationController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [HttpPost]
        [Route("call")]
        [UseProductionDb]
        [Authorize(Roles = "CEO")]

        public async Task<IActionResult> InsertCallAsync([FromBody] InsertCallRequest request)
        {
            try
            {
                var response = await _communicationService.InsertCallAsync(request);

                if (response.Success)
                {
                    return StatusCode(StatusCodes.Status201Created, response.Message);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, response.Message ?? "Error in inserting call.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
