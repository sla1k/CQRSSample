using System;
using System.Threading;
using System.Threading.Tasks;

using CQRS.Sample.Infrastructure.MongoDB.Repositories;
using CQRS.Sample.Models.Products;

using FluentValidation;

using MediatR;

namespace CQRS.Sample.Features.Products.Commands
{
	public class AddProductCommand : IRequest<Product>
	{
		/// <summary>
		///     Алиас продукта
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		///     Название продукта
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Тип продукта
		/// </summary>
		public ProductType Type { get; set; }

		public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Product>
		{
			private readonly IProductsRepository _productsRepository;

			public AddProductCommandHandler(IProductsRepository productsRepository) => _productsRepository =
				productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));

			public async Task<Product> Handle(AddProductCommand command, CancellationToken cancellationToken)
			{
				Product product = new();
				product.Alias = command.Alias;
				product.Name = command.Name;
				product.Type = command.Type;

				await _productsRepository.Add(product);
				return product;
			}
		}

		public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
		{
			public AddProductCommandValidator()
			{
				RuleFor(c => c.Name).NotEmpty();
				RuleFor(c => c.Alias).NotEmpty();
			}
		}
	}
}
