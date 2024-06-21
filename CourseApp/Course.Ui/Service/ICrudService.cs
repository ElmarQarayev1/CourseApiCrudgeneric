using System;
using Course.Ui.Resources;

namespace Course.Ui.Service
{
	public interface ICrudService
	{
		 Task  Create<TRequest>(TRequest data,string url);
        Task<PaginatedResponseResource<TResponse>> GetAllPaginated<TResponse>(string path, int page);
        Task<TResponse> Get<TResponse>(string path);
        Task Delete(string url);
	}
}

