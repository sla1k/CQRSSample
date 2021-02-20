using System.Collections.Generic;

namespace CQRS.Sample.Models
{
	public class DataWithTotal<T> where T : class
	{
		public DataWithTotal()
		{
		}

		public DataWithTotal(IEnumerable<T> items, int total)
		{
			Items = items;
			Total = total;
		}

		public DataWithTotal(IEnumerable<T> items, long total)
		{
			Items = items;
			Total = total;
		}

		public IEnumerable<T> Items { get; set; }
		public long Total { get; set; }
	}
}
