using System;
using Course.Ui.Exceptions;
using System.Net;

namespace Course.Ui.MiddleWares
{
	public class HttpExceptionMiddleware
	{
        private readonly RequestDelegate _next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException ex) when (ex.Status == HttpStatusCode.Unauthorized)
            {
               
                context.Response.Redirect("/auth/login");
            }
            catch (Exception)
            {
                
                context.Response.Redirect("/home/error");
            }
        }
    }
}

