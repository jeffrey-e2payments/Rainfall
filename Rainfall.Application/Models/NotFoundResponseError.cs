﻿using Rainfall.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rainfall.Application.Models
{
    public class NotFoundResponseError
    {
        public NotFoundResponseError(string message, string stationdId, IList<Error> errors)
        {
            Message = message;
            StationId = stationdId;
            if (errors != null)
                Errors = errors.Select(x => new BadRequestOutcome(x.propertyName, x.message + ". StationId: " + stationdId)).ToList();
        }

        [JsonPropertyName("detail")]
        public IList<BadRequestOutcome> Errors { get; protected set; }

        [JsonPropertyName("message")]
        public string Message { get; protected set; }

        [JsonIgnore]
        public string StationId { get; protected set; }

    }
}
