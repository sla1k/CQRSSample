using FluentValidation;
using FluentValidation.Validators;

using MongoDB.Bson;

namespace CQRS.Sample.Infrastructure.Validators
{
	public class BeAValidHexadecimalValidator : PropertyValidator, IBeAValidHexadecimalValidator
	{
		private readonly object _defaultValueForType;

		public BeAValidHexadecimalValidator(object defaultValueForType) : base(
			"Property must be valid hexadecimal string.") =>
			_defaultValueForType = defaultValueForType;

		protected override bool IsValid(PropertyValidatorContext context)
		{
			if (!(context.PropertyValue is string))
			{
				return false;
			}

			switch (context.PropertyValue)
			{
				case null:
				case string s when string.IsNullOrWhiteSpace(s):
				case string d when !ObjectId.TryParse(d, out ObjectId _):
					return false;
			}

			return true;
		}
	}

	public interface IBeAValidHexadecimalValidator : IPropertyValidator
	{
	}

	public static class BeAValidHexadecimalValidatorExtensinos
	{
		public static IRuleBuilderOptions<T, TProperty> BeAValidHexadecimal<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder) =>
			ruleBuilder.SetValidator(new BeAValidHexadecimalValidator(default(TProperty)));
	}
}
