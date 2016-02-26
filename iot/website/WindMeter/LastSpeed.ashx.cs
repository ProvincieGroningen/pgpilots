using System;
using System.Web;

namespace WindMeter
{
    public class LastSpeed : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Global.LastReceivedWindMeasurement == null
                ? GetRandomValue()
                : Global.LastReceivedWindMeasurement.Speed);
        }

        private static int? RandomValue;

        static int GetRandomValue()
        {
            RandomValue = Math.Abs(RandomValue ?? new Random().Next(1, 16)) + 2 - new Random().Next(1, 4);
            if (RandomValue < 0)
                RandomValue = 0;
            return RandomValue.Value;
        }

        public bool IsReusable => false;
    }
}