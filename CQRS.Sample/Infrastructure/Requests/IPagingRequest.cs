using Microsoft.AspNetCore.Mvc;

namespace CQRS.Sample.Infrastructure.Requests
{
	public interface IPagingRequest
	{
		/// <summary>
		///     Размер страницы
		/// </summary>
		[FromQuery]
		public int PageSize { get; set; }

		/// <summary>
		///     Номер страницы
		/// </summary>
		[FromQuery]
		public int PageIndex { get; set; }
	}
}
