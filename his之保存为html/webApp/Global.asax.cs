using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace webApp
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MvcHandler.DisableMvcResponseHeader = true;
            RemoveWebFormEngines();
        }
        protected void Application_PreSendRequestHeaders()
        {
            //删除响应头Server属性
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
        }
        //移除无用的WebForm视图引擎
        private void RemoveWebFormEngines()
        {
            var _ve = ViewEngines.Engines;
            var _wve = _ve.OfType<WebFormViewEngine>().FirstOrDefault();
            if (_wve != null)
            {
                _ve.Remove(_wve);
            }
        }
    }
}