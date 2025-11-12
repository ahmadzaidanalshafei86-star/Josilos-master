using ES.Web.Services;

namespace ES.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiKeyScheme")]
    public class DeclerationsController : ControllerBase
    {
        private readonly DeclerationsService _declerationsService;

        public DeclerationsController(DeclerationsService declerationsService)
        {
            _declerationsService = declerationsService;
        }


        // GET: api/declerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SilosDeclerations>>> GetAll()
        {
            var declarations = await _declerationsService.GetAllAsync();
            return Ok(declarations);
        }

        // GET: api/declerations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SilosDeclerations>> GetById(int id)
        {
            var declaration = await _declerationsService.GetByIdAsync(id);

            if (declaration == null)
                return NotFound();

            return Ok(declaration);
        }

        // POST: api/declerations
        [HttpPost]
        public async Task<ActionResult<SilosDeclerations>> Create([FromBody] SilosDeclerations declaration)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _declerationsService.CreateAsync(declaration);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // POST: api/declerations/update/{id}
        [HttpPost("update/{id}")]
        public async Task<ActionResult<SilosDeclerations>> Update(int id, [FromBody] SilosDeclerations declaration)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _declerationsService.UpdateAsync(id, declaration);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // POST: api/declerations/delete/{id}
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _declerationsService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
