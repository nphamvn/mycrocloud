using System;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services
{
	public class ExpectionCallbackExecutor: IRequestHandler
	{
		public ExpectionCallbackExecutor()
		{
		}

        public async Task<AppResponse> Handle(AppRequest request)
        {
            //1: Get the Class that implements ExpectionCallback interface

            //2: Read and 
        }
    }
}

