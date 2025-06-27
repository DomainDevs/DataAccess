using Microsoft.AspNetCore.Mvc;
using SolucionDA.Models;
using SolucionDA.UnitOfWork;
using System.Diagnostics;

namespace SolucionDA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RamoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RamoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IEnumerable<tramo>> Get()
        {
            var repo = _unitOfWork.GetRepository<tramo>("Sybase");
            return await repo.GetAllAsync();
        }
    }
}
