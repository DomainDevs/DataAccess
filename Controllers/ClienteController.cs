using SolucionDA.Models;
using SolucionDA.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SolucionDA.Controllers;


[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IEnumerable<Cliente>> Get()
    {
        var repo = _unitOfWork.GetRepository<Cliente>("SqlServer");
        return await repo.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var key = new Dictionary<string, object> { { "Id", id } };
        var repo = _unitOfWork.GetRepository<Cliente>("SqlServer");
        var cliente = await repo.GetByIdAsync(key);

        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Cliente cliente)
    {
        var repo = _unitOfWork.GetRepository<Cliente>("SqlServer");
        await repo.InsertAsync(cliente);
        _unitOfWork.Commit("SqlServer");

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] Cliente cliente)
    {
        if (cliente.Id != id)
            return BadRequest();

        var repo = _unitOfWork.GetRepository<Cliente>("SqlServer");
        await repo.UpdateAsync(cliente);
        _unitOfWork.Commit("SqlServer");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var key = new Dictionary<string, object> { { "Id", id } };
        var repo = _unitOfWork.GetRepository<Cliente>("SqlServer");
        await repo.DeleteAsync(key);
        _unitOfWork.Commit("SqlServer");

        return NoContent();
    }
}