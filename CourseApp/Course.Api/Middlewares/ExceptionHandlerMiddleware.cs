using System;
using Course.Service.Exceptions;

namespace Course.Api.Middlewares
{
	public class ExceptionHandlerMiddleware
	{
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
		{
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
           
            }
            catch(Exception ex)
            {
                var message = ex.Message;
                var errors = new List<RestExceptionError>();
                context.Response.StatusCode = 500;

                if(ex is RestException rest)
                {
                    message = rest.Message;
                    errors = rest.Errors;
                    context.Response.StatusCode = rest.Code;

                }
                await context.Response.WriteAsJsonAsync(new { message, errors });
            }
        }
	}
}

