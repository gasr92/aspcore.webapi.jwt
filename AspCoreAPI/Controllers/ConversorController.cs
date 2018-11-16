using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AspCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize("Bearer")]
    public class ConversorController :Controller
    {
        [HttpGet("PesMetros/{alturaPes}")]
        public object Get(double alturaPes)
        {
            var obj = new
            {
                AlturaPes = alturaPes,
                AlturaMetros = Math.Round(alturaPes * 0.3048,4),
            };

            return obj;
        }
    }
}