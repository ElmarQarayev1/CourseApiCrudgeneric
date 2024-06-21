using System;
namespace Course.Service.Exceptions
{
	public class DublicateEntityException:Exception
	{
		public DublicateEntityException(string message):base(message)
		{
		}
		public DublicateEntityException()
		{

		}
	}
}

