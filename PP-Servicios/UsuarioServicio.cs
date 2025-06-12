using PP_Dominio.Entidades;
using PP_Infraestructura;

namespace PP_Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IRepositorio<Usuario> repositorio;
        private DataContext dataContext;

        public UsuarioServicio(IRepositorio<Usuario> repositorio, DataContext dataContext)
        {
            this.repositorio = repositorio;
            this.dataContext = dataContext;
        }

        #region Metodos de busqueda
       
        public async Task<IEnumerable<Usuario>> GetAll()
        {
            var usuarios = await this.repositorio.GetAll();

            return usuarios.Select(x => new Usuario
            {
                Id = x.Id,
                EstaBorrado = x.EstaBorrado,
                RowVersion = x.RowVersion,
                UserName = x.UserName,
                Password = x.Password
            
            }).ToList();
        }
        #endregion

        #region Metodos de persistencia

        public async Task Create(Usuario entity)
        {
            Usuario usuario = new Usuario
            {
                UserName = entity.UserName,
                Password = entity.Password,

                EstaBorrado = false
            };

            await repositorio.Create(usuario);
        }

        public async Task Update(int id, Usuario entity)
        {
            Usuario usuario = dataContext.Usuario.FirstOrDefault(x => x.Id == id);

            usuario.UserName = entity.UserName;
            usuario.Password = entity.Password;
            
            await repositorio.Update(usuario);
        }
        
        public async Task Delete(int id)
        {
            Usuario usuario = dataContext.Usuario.FirstOrDefault(x => x.Id == id);

            await repositorio.Delete(usuario);
        }

        #endregion

        #region Metodos de verificacion

        public bool VeryfyUserNameExists(string userName)
        {
            return dataContext.Usuario.Any(x => x.UserName == userName);
        }

        public bool VeryfyIdExists(int id)
        {
            return dataContext.Usuario.Any(x => x.Id == id);
        }

        #endregion
    }

    public interface IUsuarioServicio
    {
        Task Create(Usuario entity);
        Task Update(int id, Usuario entity);
        Task Delete(int id);
        Task<IEnumerable<Usuario>> GetAll();
        bool VeryfyUserNameExists(string userName);
        bool VeryfyIdExists(int id);
    }
}
