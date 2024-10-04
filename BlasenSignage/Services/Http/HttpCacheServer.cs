using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BlasenSignage.Services.Http
{
    public class HttpCacheServer : HttpServer
    {
        public HttpCacheServer(IPAddress address, int port)
            : base(address, port)
        {
        }


        protected override TcpSession CreateSession()
        {
            return new HttpCacheSession(this);
        }


        protected override void OnError(SocketError error)
        {
            Debug.WriteLine($"HTTP session caught an error: {error}");
        }
    }
}
