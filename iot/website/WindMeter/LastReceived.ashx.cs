using System;
using System.Web;

namespace WindMeter
{
    public class LastReceived : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Math.Round(DateTime.Now.Subtract(Global.LastReceived).TotalSeconds, 1));
        }

        public bool IsReusable => false;
    }
}