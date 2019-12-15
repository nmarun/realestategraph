using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealEstate.API.Models;

namespace RealEstate.API.Controllers
{
    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly ILogger<GraphQLController> _logger;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter, ILogger<GraphQLController> logger)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs, 
                ThrowOnUnhandledException = true
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);
            _logger.LogInformation(JsonConvert.SerializeObject(result.Perf));

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
