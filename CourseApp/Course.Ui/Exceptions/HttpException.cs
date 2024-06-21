using System;
using Course.Ui.Resources;
using System.Net;

namespace Course.Ui.Exceptions
{
	public class HttpException:Exception
	{
        public HttpStatusCode Status { get; set; }
        public ErrorResponse Error { get; set; }


        public HttpException(HttpStatusCode status)
        {
            this.Status = status;
        }

        public HttpException(HttpStatusCode status, ErrorResponse error)
        {
            this.Status = status;
            this.Error = error;
        }
    }
}

