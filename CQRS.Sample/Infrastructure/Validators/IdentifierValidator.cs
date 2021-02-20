using CQRS.Sample.Infrastructure.Requests;

using FluentValidation;

namespace CQRS.Sample.Infrastructure.Validators
{
	public class IdentifierValidator<T> : AbstractValidator<T> where T : IIdentifiedRequest
	{
		public IdentifierValidator() => RuleFor(q => q.Id).NotEmpty().BeAValidHexadecimal();
	}
}
