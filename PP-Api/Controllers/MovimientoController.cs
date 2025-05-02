using Microsoft.AspNetCore.Mvc;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
using PP_Infraestructura;
using PP_Servicios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PP_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly IBilleteraServicio billeteraServicio;
        private readonly IMovimientoServicio movimientoServicio;
        private readonly DataContext dataContext;

        public MovimientoController(IMovimientoServicio movimientoServicio, IBilleteraServicio billeteraServicio, DataContext dataContext)
        {
            this.movimientoServicio = movimientoServicio;
            this.billeteraServicio = billeteraServicio;
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
                IEnumerable<Movimiento> movimientos = await movimientoServicio.GetAll();
                return Ok(new { total = movimientos.Count(), movimientos });
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
            Movimiento movimiento = new Movimiento
            {
                WalletId = model.WalletId,
                Amount = model.Amount,
                Type = model.Type
            };

            await movimientoServicio.Create(movimiento);
            return Ok(new { mensaje = "Movimiento creado correctamente" });
        }

        #endregion

        #region Metodos de transferencia

        [HttpPost]
        [Route("posttransferencia")]
        public async Task<IActionResult> PostTransferencia(string documentId, string name, [FromBody] MovimientoModel model)
        {
            if (!movimientoServicio.VerifyWalletExist(documentId, name))
                return NotFound(new { mensaje = "No existe la billetera a la que quiere transferir" });

            if (!movimientoServicio.VerifyAmountGreaterThanOrEqualToBalance(documentId, name, model.Amount))
                return NotFound(new { mensaje = "El monto a transferir supera el que tiene en la billetera." });

            if (movimientoServicio.VerifyNegativeAmountOrZero(model.Amount))
                return NotFound(new { mensaje = "El monto a transferir no puede ser cero o negativo" });

            Movimiento movimiento = new Movimiento
            {
                WalletId = model.WalletId,
                Amount = model.Amount,
                Type = model.Type
            };

            await movimientoServicio.Create(movimiento);
            await billeteraServicio.UpdateWalletForMovimiento(movimiento);
            
            return Ok(new { mensaje = "Transferencia realizada correctamente" });
        }

        #endregion
    }
}
