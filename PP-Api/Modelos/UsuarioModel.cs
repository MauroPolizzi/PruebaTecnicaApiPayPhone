using FluentValidation;

namespace PP_Api.Modelos
{
    public class UsuarioModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UsuarioModelValidator : AbstractValidator<UsuarioModel>
    {
        public UsuarioModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("El campo UserName es requerido");
            RuleFor(x => x.Password).NotEmpty().WithMessage("El campo Password es requerido");
        }
    }
}
