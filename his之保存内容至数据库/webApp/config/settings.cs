using System;
using UNIT;

namespace webApp
{
    public class settings
    {
        #region 私有成员
        private static readonly Lazy<settings> _instance = new Lazy<settings>(() =>
        {
            return XmlConfig.getConfig<settings>("settings.config");
        });
        public static settings instance
        {
            get { return _instance.Value; }
        }
        #endregion

        public string Version { get; set; }

        public string CookieVal { get; set; }
        public int RedisDbNum { get; set; }
        public string LoginUrl { get; set; }
    }
}