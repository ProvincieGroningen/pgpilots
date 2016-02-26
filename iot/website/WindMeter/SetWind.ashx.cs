using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindMeter
{
    /// <summary>
    /// Summary description for SetWind
    /// </summary>
    public class SetWind : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["reset"] != null)
            {
                Global.LastReceivedWindMeasurement = null;
            }
            else
            {
                Global.LastReceivedWindMeasurement = Global.LastReceivedWindMeasurement ?? new WindMeasurement();
                Global.LastReceivedWindMeasurement.Direction = decimal.Parse(context.Request["richting"]);
                Global.LastReceivedWindMeasurement.Speed = decimal.Parse(context.Request["snelheid"]);
                Global.LastReceived = DateTime.Now;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}