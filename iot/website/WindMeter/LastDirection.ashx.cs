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
                ? GetRandomValue()
                : (int) Global.LastReceivedWindMeasurement.Direction);
        }

        private static int? RandomValue;

        static int GetRandomValue()
        {
            RandomValue = Math.Abs((RandomValue ?? new Random().Next(1, 10)) + 5 - new Random().Next(1, 10));
            if (RandomValue < 0)
                RandomValue = 0;
            return RandomValue.Value;
        }

        public bool IsReusable => false;
    }
}