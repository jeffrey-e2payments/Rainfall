﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rainfall.Application.Models;
using Rainfall.Application.Queries;
using Rainfall.SharedLibrary.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Rainfall.Web.API.Controllers
{
    [Route("")]
    public class RainfallController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RainfallController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get rainfall readings by station Id
        /// </summary>
        /// <param name="stationId">The id of the reading station</param>
        /// <param name="count">The number of readings to return</param>
        /// <returns></returns>
        [HttpGet("/rainfall/id/{stationId}/readings")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetReadingByStationIdCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationResponseError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResponseError))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(InternalPaymentsResponseError))]
        public async Task<IActionResult> Readings([FromRoute] string stationId, [FromQuery] int count = 10)
        {
            try
            {
                var response = await _mediator.Send(new GetReadingByStationIdCommand()
                {
                    StationId = stationId,
                    Count = count
                });

                if (response != null)
                    return Ok(response);

                return Ok();
            }
            catch (RainfallException re)
            {
                switch (re)
                {
                    case RainfallRecordValidationException rve: return base.BadRequest(new ValidationResponseError(stationId, rve.Errors));
                    case RainfallRecordNotFoundException rnf: return base.NotFound(new NotFoundResponseError(stationId, rnf.Errors));
                    default: break;
                }
            }
            catch(Exception e)
            {
                // to do: log error
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new InternalPaymentsResponseError(stationId));
        }
    }
}
