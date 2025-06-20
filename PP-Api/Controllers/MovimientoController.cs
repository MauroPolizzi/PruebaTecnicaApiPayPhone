using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP_Api.Messages.AuthError;
using PP_Api.Messages.MovimientoMessage;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
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

        public MovimientoController(IMovimientoServicio movimientoServicio, IBilleteraServicio billeteraServicio, IMapper mapper)
        {
            this.movimientoServicio = movimientoServicio;
            this.billeteraServicio = billeteraServicio;
            this.mapper = mapper;
        }


        #region Metodos de busqueda

        // GET: api/<MovimientoController>/getmovimientos
        [HttpGet("getmovimientos")]
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
                return BadRequest( MovimientoMessageError.MessageCollectionEmpty() );
            }
        }

        #endregion

        #region Metodos de persistencia

        // POST api/<MovimientoController>/postmovimiento
        [HttpPost("postmovimiento")]
        public async Task<IActionResult> Post([FromBody] MovimientoModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            Movimiento movimiento = mapper.Map<Movimiento>(model);

            await movimientoServicio.Create(movimiento);
            return Ok(new { mensaje = MovimientoMessageOK.MessageCreateOk() });
        }

        #endregion

        #region Metodos de transferencia

        // POST api/<MovimientoController>/posttransferencia?documentId={""}&name={""}
        [HttpPost("posttransferencia")]
        public async Task<IActionResult> PostTransferencia(string documentId, string name, [FromBody] MovimientoModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            if (!movimientoServicio.VerifyWalletExist(documentId, name))
                return NotFound(new { mensaje = MovimientoMessageError.MessageWalletExist() });

            if (!movimientoServicio.VerifyAmountGreaterThanOrEqualToBalance(documentId, name, model.Amount))
                return NotFound(new { mensaje = MovimientoMessageError.MessageAmountGreaterThanOrEqualToBalance() });

            if (movimientoServicio.VerifyNegativeAmountOrZero(model.Amount))
                return NotFound(new { mensaje = MovimientoMessageError.MessageNegativeAmountOrZero() });

            Movimiento movimiento = mapper.Map<Movimiento>(model);

            await movimientoServicio.Create(movimiento);
            await billeteraServicio.UpdateWalletForMovimiento(movimiento);
            
            return Ok(new { mensaje = MovimientoMessageOK.MessageCreateTransferOk() });
        }

        #endregion
    }
}
