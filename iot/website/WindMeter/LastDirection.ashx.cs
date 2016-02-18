using System;
using System.Web;

namespace WindMeter
{
    public class LastDirection : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Global.LastReceivedWindMeasurement == null
                ? new Random().Next(1, 16)
                : (int) Global.LastReceivedWindMeasurement.Direction);
        }

        public bool IsReusable => false;
    }
}