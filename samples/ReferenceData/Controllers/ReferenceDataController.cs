using Microsoft.AspNetCore.Mvc;
using Kaleido;
using Kaleido.Metadata;

namespace PriorAuthIntake.ReferenceData.Api.Controllers;

[ApiController]
[Route("v1")]
public class ReferenceDataController : ControllerBase
{
    private readonly IValueSetCatalog _catalog;

    public ReferenceDataController(IValueSetCatalog catalog
        //, IMapper mapper
        )
    {
        _catalog = catalog;
        //_mapper = mapper;
    }
    [HttpGet("value-sets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<ValueSetMetadata>> GetValueSets()
    {
        var metas = _catalog.GetAll();
        //var mapped = metas.Select(m => _mapper.Map<ValueSetMetadata>(m));
        return Ok(metas);
    }


    [HttpGet("value-sets/{valueSetKey}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ValueSetMetadata> GetValueSetMetadata(string valueSetKey)
    {
        var meta = _catalog.Get(valueSetKey);
        if (meta == null) return NotFound();
        //var mapped = _mapper.Map<ValueSetMetadata>(meta);
        return Ok(meta);
    }

    [HttpPost("value-sets/{valueSetKey}/query")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ValueSetQueryResponse> QueryValueSet(string valueSetKey, [FromBody] QueryRequest request)
    {
        try
        {
            var response = _catalog.QueryAsync(valueSetKey, request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            var error = new
            {
                error = new
                {
                    code = "FILTER_NOT_SUPPORTED",
                    message = ex.Message,
                    details = new[] { new { field = "filters", reason = "One or more filters are not supported for this value set." } },
                    allowedFilters = new[] { "active", "clientId", "groupName", "clientName", "includeAllClientsOption" }
                }
            };
            return BadRequest(error);
        }
    }

    //[HttpGet("value-sets/{valueSetKey}/values/{valueId}")]
    //public IActionResult GetValue(string valueSetKey, string valueId)
    //{
    //    // Simple stub that returns a resolved item
    //    var item = new ValueItem(valueId, valueId.Split(':').Last(), null, "Resolved", null, null, true, true, 10, null, null, null);
    //    return Ok(item);
    //}

    //[HttpPost("resolve")]
    //public IActionResult Resolve([FromBody] ResolveRequest request)
    //{
    //    var items = request.Items.Select(i => new ResolvedItem(i.ItemKey, "FOUND", i.ValueSetKey, i.CodeSystem, i.Code, i.Code, "", true, i.Version ?? "")).ToList();
    //    var resp = new ResolveResponse(DateTime.UtcNow, items, new List<object>());
    //    return Ok(resp);
    //}

    //[HttpPost("batch-query")]
    //public IActionResult BatchQuery([FromBody] BatchQueryRequest request)
    //{
    //    var results = new Dictionary<string, QueryResponse>();
    //    foreach (var q in request.Queries)
    //    {
    //        // reuse single-query behavior by returning empty value-set metadata
    //        results[q.Alias] = new QueryResponse(new ValueSetMetadata(q.ValueSetKey, null, null, null, null, q.QueryName, null, null, null, null, null, null, null, "2026.07.14.1", DateTime.UtcNow.Date, null), q.Query, new QueryResult(0, q.Query?.Page?.Limit ?? 50, false, null), new List<ValueItem>(), new List<object>());
    //    }

    //    return Ok(new BatchQueryResponse(results, new List<object>()));
    //}
}
