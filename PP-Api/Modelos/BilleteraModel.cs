using FluentValidation;

namespace PP_Api.Modelos
{
    public class BilleteraModel
    {
        public string DocumentId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BilleteraModelValidator : AbstractValidator<BilleteraModel>
    {
        public BilleteraModelValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty().WithMessage("El campo Documento es requerido")
                .MinimumLength(5).WithMessage("El campo Documento debe tener por lo menos 5 caracteres");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El campo Nombre debe ser valido")
                .MinimumLength(2).WithMessage("El campo Nombre debe tener por lo menos 2 caracteres");

            RuleFor(x => x.Balance)
                .NotNull().WithMessage("El campo Balance no puede ser null")
                .NotEmpty().WithMessage("El campo Balance es requerido")
                .GreaterThanOrEqualTo(0).WithMessage("El campo Balance no puede ser negativo");
        }
    }
}
