using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP_Api.AuthError;
using PP_Api.BilleteraMessage;
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
    public class BilleteraController : ControllerBase
    {
        private readonly IBilleteraServicio billeteraServicio;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;

        public BilleteraController(IBilleteraServicio billeteraServicio, IMapper mapper, DataContext dataContext)
        {
            this.billeteraServicio = billeteraServicio;
            this.mapper = mapper;
            this.dataContext = dataContext;
        }

        #region Metodos de busqueda

        // GET: api/<BilleteraController>
        [HttpGet]
        [Route("getbilletera")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Validamos la autenticidad del token
                if(!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

                IEnumerable<Billetera> billeteras = await billeteraServicio.GetAll();
                
                IEnumerable<BilleteraModel> billeteraModels = mapper.Map<IEnumerable<BilleteraModel>>(billeteras);
                
                return Ok(new { total = billeteraModels.Count(), billeteraModels });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch
            {
                return BadRequest(BilleteraMessageError.MessageCollectionEmpty());
            }
        }

        #endregion

        #region Metodos de persistencia

        // POST api/<BilleteraController>
        [HttpPost]
        [Route("postbilletera")]
        public async Task<IActionResult> Post([FromBody] BilleteraModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos
            if (billeteraServicio.VerifyDocumentOrNameExists(model.DocumentId, model.Name))
                return NotFound(new { mensaje = BilleteraMessageError.MessageDocumentOrNameExists() });

            Billetera billetera = mapper.Map<Billetera>(model);

            await billeteraServicio.Create(billetera);
            return Ok(new { mensaje = BilleteraMessageOk.MessageCreateOK() });
        }

        // PUT api/<BilleteraController>/putbilletera/5
        [HttpPut("{id}")]
        [Route("putbilletera")]
        public async Task<IActionResult> Put(int id, [FromBody] BilleteraModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos
            if (!billeteraServicio.VerifyIdExist(id))
                return NotFound(new { mensaje = BilleteraMessageError.MessageExists() });

            if (billeteraServicio.VerifyBalanceNegative(model.Balance))
                return NotFound(new { mensaje = BilleteraMessageError.MessageBalanceNegative() });

            Billetera billetera = mapper.Map<Billetera>(model);

            await billeteraServicio.Update(id, billetera);
            return Ok(new { mensaje = BilleteraMessageOk.MessageUpdateOK() });
        }

        // DELETE api/<BilleteraController>/5
        [HttpDelete("{id}")]
        [Route("deletebilletera")]
        public async Task<IActionResult> Delete(int id)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos
            if (!billeteraServicio.VerifyIdExist(id))
                return NotFound(new { mensaje = BilleteraMessageError.MessageExists() });

            await billeteraServicio.Delete(id);
            return Ok(new { mensaje = BilleteraMessageOk.MessageDeleteOK() });
        }

        #endregion
    }
}
