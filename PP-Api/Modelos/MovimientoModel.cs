using FluentValidation;
using PP_Dominio.Entidades;

namespace PP_Api.Modelos
{
    public class MovimientoModel
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TypeOperation Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MovimientoModelValidator : AbstractValidator<MovimientoModel>
    {
        public MovimientoModelValidator()
        {
            RuleFor(x => x.WalletId)
                .NotEmpty().WithMessage("El campo WalletId es requerido")
                .NotEqual(0).WithMessage("El campo WalletId no puede ser cero");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("El campo Amount es requerido")
                .GreaterThanOrEqualTo(0).WithMessage("El campo Amount no puede ser cero o negativo");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El campo Type es requerido")
                .NotEqual(TypeOperation.None).WithMessage("El campo Type no puede ser cero")
                .IsInEnum().WithMessage("El campo Type proporcionado no es valido");
        }
    }
}
