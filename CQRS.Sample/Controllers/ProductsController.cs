using System;
using System.Threading;
using System.Threading.Tasks;

using CQRS.Sample.Features.Products.Commands;
using CQRS.Sample.Features.Products.Queries;
using CQRS.Sample.Models;
using CQRS.Sample.Models.Products;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CQRS.Sample.Controllers
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly ILogger<ProductsController> _logger;
		private readonly IMediator _mediator;

		public ProductsController(IMediator mediator) =>
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

		/// <summary>
		///     Постраничное получение продуктов
		/// </summary>
		/// <param name="request"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		[HttpGet]
		[ProducesResponseType(typeof(DataWithTotal<Product>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesDefaultResponseType]
		public async Task<ActionResult<DataWithTotal<Product>>> Get([FromQuery] GetProductsQuery request,
			CancellationToken token) =>
			Ok(await _mediator.Send(request, token));

		/// <summary>
		///     Создание продукта
		/// </summary>
		/// <param name="client"></param>
		/// <param name="apiVersion"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		[HttpPost]
		[ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> Post([FromBody] AddProductCommand client, ApiVersion apiVersion,
			CancellationToken token)
		{
			Product entity = await _mediator.Send(client, token);
			return CreatedAtAction(nameof(Get), new {id = entity.Id, version = apiVersion.ToString()}, entity);
		}
	}
}
