using NetCoreServer;
using System.Net.Sockets;

namespace BlasenSignage.Services.Http
{
    public class HttpCacheSession : HttpSession
    {
        public HttpCacheSession(HttpServer server)
            : base(server)
        {
        }


        protected override void OnReceivedRequest(HttpRequest request)
        {
            if (request.Method == "HEAD")
            {
                SendResponseAsync(Response.MakeHeadResponse());
            }
            else if (request.Method == "GET")
            {
                var key = request.Url;

                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

                if (string.IsNullOrEmpty(key))
                {
                    SendResponseAsync(Response.MakeGetResponse(CommonCache.GetInstance().GetAllCache(), "application/json; charset=UTF-8"));
                }
                else if (CommonCache.GetInstance().GetCacheValue(key, out var value))
                {
                    SendResponseAsync(Response.MakeGetResponse(value));
                }
                else
                    SendResponseAsync(Response.MakeErrorResponse(404, "Required cache value was not found for the key" + key));
            }
            else if ((request.Method == "POST") || (request.Method == "PUT"))
            {
                var key = request.Url;
                var value = request.Body;

                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

                CommonCache.GetInstance().PutCacheValue(key, value);

                SendResponseAsync(Response.MakeOkResponse());
            }
            else if (request.Method == "DELETE")
            {
                var key = request.Url;

                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

                if (CommonCache.GetInstance().DeleteCacheValue(key, out var value))
                {
                    SendResponseAsync(Response.MakeGetResponse(value));
                }
                else
                    SendResponseAsync(Response.MakeErrorResponse(404, "Deleted cache value was not found for the key: " + key));
            }
            else if (request.Method == "OPTIONS")
                SendResponseAsync(Response.MakeOptionsResponse());
            else if (request.Method == "TRACE")
                SendResponseAsync(Response.MakeTraceResponse(request.Cache.Data));
            else
                SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
        }



        protected override void OnReceivedRequestError(HttpRequest request, string error)
        {
        }


        protected override void OnError(SocketError error)
        {
        }
    }
}
