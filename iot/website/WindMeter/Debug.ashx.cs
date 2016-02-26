using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindMeter
{
    /// <summary>
    /// Summary description for Debug
    /// </summary>
    public class Debug : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Global.Debug);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}