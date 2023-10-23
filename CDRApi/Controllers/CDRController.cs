using AutoMapper;
using CDRApi.Messages;
using CDRApi.Model;
using CDRApi.Services;
using CDRModel;
using CDRServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CDRApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CDRController : ControllerBase
    {
        private readonly ILogger<CDRController> _logger;
        private readonly ICDRRepository _repository;
        private readonly IMapper _mapper;

        public CDRController(ILogger<CDRController> logger, ICDRRepository repository, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(repository, nameof(repository));
            ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));

            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> Post([Required] CallDto[] calls, CancellationToken cancellation)
        //{
        //    var transformedCalls = calls.Select(x => _mapper.Map<CallDto, Call>(x)).ToList();

        //    _logger.LogInformation($"Uploaded {transformedCalls.Count} calls");

        //    var results = await _repository.Save(transformedCalls, cancellation);

        //    _logger.LogInformation($"Inserted {results} calls into the database");

        //    return Ok();
        //}

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([Required] IFormFile file, [FromServices] ICsvParser parser, IMediator mediator, CancellationToken cancellation)
        {
            try
            {
                var calls = await parser.Parse(file, cancellation);

                if (!calls.Any())
                {
                    return BadRequest();
                }

                var transformedCalls = calls.Select(x => _mapper.Map<CallDto, Call>(x)).ToList();

                _logger.LogInformation($"Uploaded {transformedCalls.Count} calls");

                var results = await _repository.Save(transformedCalls, cancellation);

                _logger.LogInformation($"Inserted {results} calls into the database");

                await mediator.Publish(new FileUploadedCommand(transformedCalls), cancellation);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        [HttpGet("{reference}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CallDto>> Get(string reference, CancellationToken cancellation)
        {
            try
            {
                var call = await _repository.Find(reference, cancellation);

                if (call != null)
                {
                    var callDto = _mapper.Map<Call, CallDto>(call);

                    return Ok(callDto);
                }

                _logger.LogInformation($"Call with reference {reference} not found");

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while trying to find a call by reference");
                throw;
            }
        }

        [HttpGet("[action]/{from}/{to}/{type?}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CallStatsDto>> CallStats(CancellationToken cancellation, DateTime from, DateTime to, CallType? type = null)
        {
            if (from > to)
            {
                _logger.LogInformation($"Trying to get statistics for an invalid time period");
                return BadRequest();
            }

            if (to - from > TimeSpan.FromDays(31))
            {
                _logger.LogInformation($"The requested time period is too big");
                return BadRequest();
            }

            try
            {
                var callStats = await _repository.CallStats(from, to, type, cancellation);

                if (callStats == null)
                {
                    _logger.LogInformation("No records found");
                    return NotFound();
                }

                var callStatsDto = _mapper.Map<CallStats, CallStatsDto>(callStats);

                _logger.LogInformation($"Got {callStatsDto.TotalCount} calls that lasted {callStatsDto.TotalDuration} from {from} until {to} of type {type}");

                return Ok(callStatsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving call statistics");
                throw;
            }
        }

        [HttpGet("[action]/{callerId}/{from}/{to}/{type?}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CallerStatsDto>>> CallerStats(CancellationToken cancellation, string callerId, DateTime from, DateTime to, CallType? type = null)
        {
            if (from > to)
            {
                _logger.LogInformation($"Trying to get statistics for an invalid time period");
                return BadRequest();
            }

            if (to - from > TimeSpan.FromDays(31))
            {
                _logger.LogInformation($"The requested time period is too big");
                return BadRequest();
            }

            try
            {
                var callerStats = await _repository.CallerStats(callerId, from, to, type, cancellation);

                if (callerStats == null)
                {
                    _logger.LogInformation("No records found");
                    return NotFound();
                }

                var callStatsDto = callerStats.Select(x => _mapper.Map<CallerStats, CallerStatsDto>(x)).ToList();

                _logger.LogInformation($"Retrieved {callStatsDto.Count} records for caller {callerId} from {from} until {to} for type {type}");

                return Ok(callStatsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving caller statistics");
                throw;
            }
        }

        [HttpGet("[action]/{callerId}/{top}/{from}/{to}/{type?}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CallerStatsDto>>> TopCallStats(CancellationToken cancellation, string callerId, int top, DateTime from, DateTime to, CallType? type = null)
        {
            if (from > to)
            {
                _logger.LogInformation($"Trying to get statistics for an invalid time period");
                return BadRequest();
            }

            if (to - from > TimeSpan.FromDays(31))
            {
                _logger.LogInformation($"The requested time period is too big");
                return BadRequest();
            }

            if (top < 0 || top > 10)
            {
                _logger.LogInformation($"The requested number of records is invalid");
                return BadRequest();
            }

            try
            {
                var topCallStats = await _repository.TopCallStats(callerId, top, from, to, type, cancellation);

                if (topCallStats == null)
                {
                    _logger.LogInformation("No records found");
                    return NotFound();
                }

                var topCallStatsDto = topCallStats.Select(x => _mapper.Map<CallerStats, CallerStatsDto>(x)).ToList();

                _logger.LogInformation($"Retrieved {topCallStatsDto.Count} records for caller {callerId} from {from} until {to} for type {type}");

                return Ok(topCallStatsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top call statistics");
                throw;
            }
        }
    }
}