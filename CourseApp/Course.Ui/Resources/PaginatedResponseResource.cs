using System;
namespace Course.Ui.Resources
{
	public class PaginatedResponseResource<T>
	{
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
    }
}

