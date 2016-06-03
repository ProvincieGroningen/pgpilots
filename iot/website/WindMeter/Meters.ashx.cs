using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WindMeter
{
    public class Meters : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(Global.Nodes.Select(n => new {NodeEui = n.Key, NodeDescription = n.Value})));
        }

        public bool IsReusable => false;
    }
}