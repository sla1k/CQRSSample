using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using CQRS.Sample.Infrastructure.MongoDB.Repositories;
using CQRS.Sample.Infrastructure.Requests;
using CQRS.Sample.Infrastructure.Validators;
using CQRS.Sample.Models;
using CQRS.Sample.Models.Products;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CQRS.Sample.Features.Products.Queries
{
	public class GetProductsQuery : IRequest<DataWithTotal<Product>>, IPagingRequest
	{
		/// <summary>
		///     Размер страницы
		/// </summary>
		[DefaultValue(10)]
		[FromQuery]
		public int PageSize { get; set; } = 10;

		/// <summary>
		///     Индекс страницы
		/// </summary>
		[DefaultValue(0)]
		[FromQuery]
		public int PageIndex { get; set; } = 0;

		public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, DataWithTotal<Product>>
		{
			private readonly ILogger<GetProductsQueryHandler> _logger;
			private readonly IProductsRepository _productsRepository;

			public GetProductsQueryHandler(IProductsRepository productsRepository,
				ILogger<GetProductsQueryHandler> logger)
			{
				_productsRepository = productsRepository;
				_logger = logger;
			}


			public async Task<DataWithTotal<Product>> Handle(GetProductsQuery query,
				CancellationToken cancellationToken)
			{
				IEnumerable<Product> products =
					await _productsRepository.Get(query.PageSize, query.PageSize * query.PageIndex);
				long total = await _productsRepository.Count();

				return new DataWithTotal<Product>(products, (int)total);
			}
		}

		public class GetProductsQueryValidator : PagingValidator<GetProductsQuery>
		{
		}
	}
}
