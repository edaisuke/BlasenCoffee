using EmbedIO;
using EmbedIO.Files;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlasenSignageManager.Services
{
    public class HttpServerService
    {
        public HttpServerService()
        {
            httpServer = CreateWebServer();
        }


        private readonly WebServer httpServer;



        private WebServer CreateWebServer()
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix("http://*:8080/")
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithStaticFolder("/", "../../../../../www/api", true, m => m.WithContentCaching(true));

            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }



        public void Start()
        {
            this.httpServer.Start();
        }


        public void Stop()
        {
            this.httpServer.Dispose();
        }
    }
}
