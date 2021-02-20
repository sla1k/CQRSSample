using System.Net;

using CQRS.Sample.Infrastructure.ActionResults;
using CQRS.Sample.Infrastructure.Exceptions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CQRS.Sample.Infrastructure.Filters
{
	public class HttpGlobalExceptionFilter : IExceptionFilter
	{
		private readonly IWebHostEnvironment env;
		private readonly ILogger<HttpGlobalExceptionFilter> logger;

		public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
		{
			this.env = env;
			this.logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			logger.LogError(new EventId(context.Exception.HResult),
				context.Exception,
				context.Exception.Message);

			if (context.Exception.GetType() == typeof(CqrsSampleDomainException))
			{
				ValidationProblemDetails problemDetails = new()
				{
					Instance = context.HttpContext.Request.Path,
					Status = StatusCodes.Status400BadRequest,
					Detail = "Please refer to the errors property for additional details."
				};

				problemDetails.Errors.Add("DomainValidations", new[] {context.Exception.Message});

				context.Result = new BadRequestObjectResult(problemDetails);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			}
			else
			{
				JsonErrorResponse json = new() {Messages = new[] {"An error occur.Try it again."}};

				if (env.IsDevelopment())
				{
					json.DeveloperMessage = context.Exception;
				}

				context.Result = new InternalServerErrorObjectResult(json);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}

			context.ExceptionHandled = true;
		}

		private class JsonErrorResponse
		{
			public string[] Messages { get; set; }

			public object DeveloperMessage { get; set; }
		}
	}
}
