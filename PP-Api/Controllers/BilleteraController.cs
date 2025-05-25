using AutoMapper;
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
                return BadRequest("No se encontro nada");
            }
        }

        #endregion

        #region Metodos de persistencia

        // POST api/<BilleteraController>
        [HttpPost]
        [Route("postbilletera")]
        public async Task<IActionResult> Post([FromBody] BilleteraModel model)
        {
            // Verificamos
            if (billeteraServicio.VerifyDocumentOrNameExists(model.DocumentId, model.Name))
                return NotFound(new { mensaje = "Existe una billetera con el mismo DocumentId o Name. Por favor cambiarlo." });

            Billetera billetera = mapper.Map<Billetera>(model);

            await billeteraServicio.Create(billetera);
            return Ok(new { mensaje = "Billetera creada correctamente" });
        }

        // PUT api/<BilleteraController>/putbilletera/5
        [HttpPut("{id}")]
        [Route("putbilletera")]
        public async Task<IActionResult> Put(int id, [FromBody] BilleteraModel model)
        {
            // Verificamos
            if (!billeteraServicio.VerifyIdExist(id))
                return NotFound(new { mensaje = "Billetera inexistente" });

            if (billeteraServicio.VerifyBalanceNegative(model.Balance))
                return NotFound(new { mensaje = "El saldo no puede ser negativo" });

            Billetera billetera = mapper.Map<Billetera>(model);

            await billeteraServicio.Update(id, billetera);
            return Ok(new { mensaje = "Billetera actualizada correctamente" });
        }

        // DELETE api/<BilleteraController>/5
        [HttpDelete("{id}")]
        [Route("deletebilletera")]
        public async Task<IActionResult> Delete(int id)
        {
            // Verificamos
            if (!billeteraServicio.VerifyIdExist(id))
                return NotFound(new { mensaje = "Billetera inexistente" });

            await billeteraServicio.Delete(id);
            return Ok(new { mensaje = "Billetera eliminada correctamente" });
        }

        #endregion
    }
}
