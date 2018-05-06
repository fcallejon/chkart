using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace chktr.Model
{
    public class ModelError
    {
        public ModelError()
        {
        }

        public ModelError(string message)
        {
            Detail = new [] { message };
        }

        /// <summary>
        /// To initialize a new <see cref="ModelError"/> and include the ModelState errors.
        /// </summary>
        /// <param name="modelState"></param>
        public ModelError(ModelStateDictionary modelState)
        {
            Detail = modelState
                .SelectMany(me => me.Value.Errors.Select(e => e.ErrorMessage))
                .ToList();
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Detail { get; set; }
    }
}