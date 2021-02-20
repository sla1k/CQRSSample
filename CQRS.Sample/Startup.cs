using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

using CQRS.Sample.Infrastructure.Behaviours;
using CQRS.Sample.Infrastructure.Filters;
using CQRS.Sample.Infrastructure.MongoDB;
using CQRS.Sample.Infrastructure.MongoDB.Base;
using CQRS.Sample.Infrastructure.MongoDB.Repositories;

using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CQRS.Sample
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<MongoDBSettings>(Configuration.GetSection("MongoDB"));
			services.AddTransient<SampleContext>();
			services.AddTransient<IProductsRepository, ProductsRepository>();

			services.AddMediatR(Assembly.GetExecutingAssembly());

			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

			services.AddCustomMVC(Configuration)
				.AddSwagger(Configuration);
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger()
					.UseSwaggerUI(options =>
					{
						// build a swagger endpoint for each discovered API version
						foreach (ApiVersionDescription description in provider.ApiVersionDescriptions
							.OrderByDescending(x => x.ApiVersion.MajorVersion)
							.ThenByDescending(x => x.ApiVersion.MinorVersion))
						{
							options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
								description.GroupName.ToUpperInvariant());
						}
					});
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}

	public static class CustomExtensionMethods
	{
		private static string XmlCommentsFilePath
		{
			get
			{
				string basePath = PlatformServices.Default.Application.ApplicationBasePath;
				string fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
				return Path.Combine(basePath, fileName);
			}
		}

		public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpContextAccessor();

			services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
				.AddFluentValidation(c =>
				{
					c.RegisterValidatorsFromAssemblyContaining<Startup>();
					// Optionally set validator factory if you have problems with scope resolve inside validators.
					// c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
				})
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					options.JsonSerializerOptions.IgnoreNullValues = true;
				});

			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				//options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
			});

			return services;
		}

		public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddVersionedApiExplorer(
				options =>
				{
					// add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
					// note: the specified format code will format the version as "'v'major[.minor][-status]"
					options.GroupNameFormat = "'v'VVV";

					// note: this option is only necessary when versioning by url segment. the SubstitutionFormat
					// can also be used to control the format of the API version in route templates
					options.SubstituteApiVersionInUrl = true;
				});

			services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

			services.AddSwaggerGen(options =>
			{
				// add a custom operation filter which sets default values
				options.OperationFilter<SwaggerDefaultValues>();

				// Set the comments path for the Swagger JSON and UI.
				options.IncludeXmlComments(XmlCommentsFilePath);

				options.AddFluentValidationRules();
			});

			return services;
		}
	}
}
