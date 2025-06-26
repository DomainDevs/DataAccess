using SolucionDA.Models;
using SolucionDA.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace SolucionDA.Controllers;



[ApiController]
[Route("[controller]")]
public class EmpleadoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpleadoController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IEnumerable<Empleado>> Get()
    {
        var repo = _unitOfWork.GetRepository<Empleado>();
        return await repo.GetAllAsync();
    }

    [HttpGet("{tipo}/{nro}")]
    public async Task<ActionResult<Empleado>> GetById(string tipo, string nro)
    {
        var repo = _unitOfWork.GetRepository<Empleado>();

        var key = new Dictionary<string, object>
            {
                { "Tipo_Documento", tipo },
                { "Nro_Documento", nro }
            };

        var empleado = await repo.GetByIdAsync(key);

        if (empleado == null)
            return NotFound();

        return Ok(empleado);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Empleado empleado)
    {
        var repo = _unitOfWork.GetRepository<Empleado>();
        await repo.InsertAsync(empleado);
        _unitOfWork.Commit();

        return CreatedAtAction(nameof(GetById), new { tipo = empleado.Tipo_Documento, nro = empleado.Nro_Documento }, empleado);
    }

    [HttpPut("{tipo}/{nro}")]
    public async Task<ActionResult> Put(string tipo, string nro, [FromBody] Empleado empleado)
    {
        if (empleado.Tipo_Documento != tipo || empleado.Nro_Documento != nro)
            return BadRequest("Los datos de la URL no coinciden con el cuerpo del objeto.");

        var repo = _unitOfWork.GetRepository<Empleado>();
        await repo.UpdateAsync(empleado);
        _unitOfWork.Commit();

        return NoContent();
    }

    [HttpDelete("{tipo}/{nro}")]
    public async Task<ActionResult> Delete(string tipo, string nro)
    {
        var key = new Dictionary<string, object>
            {
                { "Tipo_Documento", tipo },
                { "Nro_Documento", nro }
            };

        var repo = _unitOfWork.GetRepository<Empleado>();
        await repo.DeleteAsync(key);
        _unitOfWork.Commit();

        return NoContent();
    }

    [HttpGet("BuscarPorDocumentoSP")]
    public async Task<ActionResult<IEnumerable<Empleado>>> BuscarPorDocumentoSP(
        [FromQuery] string tipo,
        [FromQuery] string nro)
    {
        var repo = _unitOfWork.GetRepository<Empleado>();

        var parameters = new
        {
            Tipo_Documento = tipo,
            Nro_Documento = nro
        };

        var empleados = await repo.ExecuteStoredProcedureAsync("sp_BuscarEmpleadosPorDocumento", parameters);

        return Ok(empleados);
    }

    [HttpGet("Resumen")]
    public async Task<ActionResult<IEnumerable<EmpleadoResumenDto>>> ObtenerResumenEmpleado(
        [FromQuery] string tipo,
        [FromQuery] string nro)
    {
        var repo = _unitOfWork.GetRepository<Empleado>();
        var parameters = new
        {
            Tipo_Documento = tipo,
            Nro_Documento = nro
        };

        var resultado = await repo.ExecuteStoredProcedureAsync<EmpleadoResumenDto>(
            "sp_ResumenEmpleado", parameters
        );

        return Ok(resultado);
    }

}