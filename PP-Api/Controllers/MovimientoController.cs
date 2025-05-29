using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP_Api.AuthError;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PP_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly IBilleteraServicio billeteraServicio;
        private readonly IMovimientoServicio movimientoServicio;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;

        public MovimientoController(IMovimientoServicio movimientoServicio, IBilleteraServicio billeteraServicio, IMapper mapper, DataContext dataContext)
        {
            this.movimientoServicio = movimientoServicio;
            this.billeteraServicio = billeteraServicio;
            this.mapper = mapper;
            this.dataContext = dataContext;
        }


        #region Metodos de busqueda

        // GET: api/<MovimientoController>
        [HttpGet]
        [Route("getmovimientos")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Validamos la autenticidad del token
                if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

                IEnumerable<Movimiento> movimientos = await movimientoServicio.GetAll();

                IEnumerable<MovimientoModel> movimientoModels = mapper.Map<IEnumerable<MovimientoModel>>(movimientos);

                return Ok(new { total = movimientoModels.Count(), movimientoModels });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch
            {
                return BadRequest("No se encontro nada");
            }
        }

        #endregion

        #region Metodos de persistencia

        // POST api/<MovimientoController>
        [HttpPost]
        [Route("postmovimiento")]
        public async Task<IActionResult> Post([FromBody] MovimientoModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            Movimiento movimiento = mapper.Map<Movimiento>(model);

            await movimientoServicio.Create(movimiento);
            return Ok(new { mensaje = "Movimiento creado correctamente" });
        }

        #endregion

        #region Metodos de transferencia

        [HttpPost]
        [Route("posttransferencia")]
        public async Task<IActionResult> PostTransferencia(string documentId, string name, [FromBody] MovimientoModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            if (!movimientoServicio.VerifyWalletExist(documentId, name))
                return NotFound(new { mensaje = "No existe la billetera a la que quiere transferir" });

            if (!movimientoServicio.VerifyAmountGreaterThanOrEqualToBalance(documentId, name, model.Amount))
                return NotFound(new { mensaje = "El monto a transferir supera el que tiene en la billetera." });

            if (movimientoServicio.VerifyNegativeAmountOrZero(model.Amount))
                return NotFound(new { mensaje = "El monto a transferir no puede ser cero o negativo" });

            Movimiento movimiento = mapper.Map<Movimiento>(model);

            await movimientoServicio.Create(movimiento);
            await billeteraServicio.UpdateWalletForMovimiento(movimiento);
            
            return Ok(new { mensaje = "Transferencia realizada correctamente" });
        }

        #endregion
    }
}
