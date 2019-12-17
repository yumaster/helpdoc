using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace webApp
{
    public class dbUtil
    {
        //连接字符串
        private static readonly string connString = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
        /// <summary>
        /// 创建db连接
        /// </summary>
        /// <returns></returns>
        public static IDbConnection createDB()
        {
            return new SqlConnection(connString);
        }
    }
}