using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using UNIT;

namespace webApp
{
    public class AuthFilterAttribute : AuthorizeAttribute
    {
        #region 初始化
        public settings cf { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthFilterAttribute()
        {
            cf = settings.instance;
        }
        #endregion

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var _cook = filterContext.HttpContext.Request.Cookies[cf.CookieVal];
            if (_cook != null)
            {
                if (string.IsNullOrWhiteSpace(_cook.Value) ? true :
                    !new RedisCache(cf.RedisDbNum).Do(db => db.KeyExists(_cook.Value)))
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext fc)
        {
            HttpResponseBase res = fc.HttpContext.Response;
            if (fc.HttpContext.Request.IsAjaxRequest())
            {
                httpResult _res = new httpResult(4, "请登录");
                res.Write(JsonConvert.SerializeObject(_res));
                res.ContentType = "application/json";
                res.End();
            }
            else
            {
                res.Redirect(cf.LoginUrl, true);
            }
        }
    }
}