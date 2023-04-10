using Domain;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ClientsAPI.Controllers
{
    public class LogsController : ControllerBase
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IPayloadRepository _payloadRepository;

        public LogsController(ILogsRepository logsRepository, IPayloadRepository payloadRepository)
        {
            _logsRepository = logsRepository;
            _payloadRepository = payloadRepository;
        }

        [HttpGet]
        [Route("LogPayload")]
        [SwaggerOperation(Summary = "Get log payload in a specific date")]
        [SwaggerResponse(StatusCodes.Status200OK, "Log payload found", typeof(object))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No log payload")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requested payload for a failed log")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred")]
        public async Task<ActionResult<object>> LogPayload([FromQuery, SwaggerParameter("Specific date in UTC.", Required = true)] DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                return BadRequest("The provided UTC date cannot be in the future");
            }
            // Check if the requested payload is for a successful log
            var checkLogSuccessfulLog = await _logsRepository.IsSuccessfulLog(date);
            if (checkLogSuccessfulLog is null)
            {
                return NotFound("The specified log entry does not exist");
            }

            if (!checkLogSuccessfulLog.Value)
            {
                return BadRequest("The log you requested is for a failed request which does not have a payload");
            }

            try
            {
                var logPayload = await _payloadRepository.GetPayload(date);

                if (logPayload == null)
                {
                    return NotFound();
                }

                return Ok(logPayload);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Console.WriteLine($"Error Id: {errorId} An exception happened: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An internal server error occurred, please contact support with Erorr Id {errorId}");
            }

        }

        [HttpGet]
        [Route("Logs")]
        [SwaggerOperation(Summary = "Get log entries within a date range")]
        [SwaggerResponse(StatusCodes.Status200OK, "Log entries found", typeof(List<Log>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No log entries found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid date range")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occurred")]
        public async Task<ActionResult<List<Log>>> Logs(
            [FromQuery, SwaggerParameter("Start date for the date range in UTC.", Required = true)] DateTime startDate,
            [FromQuery, SwaggerParameter("End date for the date range in UTC.", Required = true)] DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date should be before end date");
            }

            try
            {
                var logEntries = await _logsRepository.GetLogsWithinRange(startDate, endDate);

                if (logEntries == null || !logEntries.Any())
                {
                    return NotFound();
                }

                return Ok(logEntries);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Console.WriteLine($"Error Id: {errorId} An exception happened: {ex} ");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An internal server error occurred, please contact support with Erorr Id {errorId}");
            }

        }
    }
}
