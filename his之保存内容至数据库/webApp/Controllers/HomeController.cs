using Dapper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using UNIT;

namespace webApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DocRead()
        {
            return View();
        }

        /// <summary>
        /// 获得树
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> getTree(int id = 0)
        {
            httpResult res = new httpResult();
            try
            {
                //string key = "HELPTREE",
                //       dkey = id.ToString();
                //RedisCache cache = new RedisCache(1);
                //if (await cache.Do(cdb => cdb.HashExistsAsync(key, dkey)))
                //{
                //    RedisValue value = await cache.Do(cdb => cdb.HashGetAsync(key, dkey));
                //    if (value.IsNullOrEmpty) throw new Exception("获取失败");
                //    else
                //    {
                //        res.result = JsonConvert.DeserializeObject<List<Catalog>>(value);
                //    }
                //}
                //else
                //{
                //    using (IDbConnection db = dbUtil.createDB())
                //    {
                //        string sql = "select id,name[n],url[u],convert(bit,case when b.pid is not null then 1 else 0 end) as i from mulu a left join(select pid from mulu group by pid) b on a.id=b.pid where a.pid=@id order by node";
                //        List<Catalog> list = (await db.QueryAsync<Catalog>(sql, new { id })).ToList();
                //        if (list.Count > 0)
                //        {
                //            await cache.Do(cdb => cdb.HashSetAsync(key, dkey, JsonConvert.SerializeObject(list)));
                //            await cache.Do(cdb => cdb.KeyExpireAsync(key, TimeSpan.FromDays(88)));
                //            res.result = list;
                //        }
                //        else throw new Exception("没有记录");
                //    }
                //}
                using (IDbConnection db = dbUtil.createDB())
                {
                    string sql = "select id,name[n],url[u],convert(bit,case when b.pid is not null then 1 else 0 end) as i from mulu a left join(select pid from mulu group by pid) b on a.id=b.pid  where a.pid=@id and a.isdel=0 order by node";
                    List<Catalog> list = (await db.QueryAsync<Catalog>(sql, new { id })).ToList();
                    if (list.Count > 0)
                    {
                        res.result = list;
                    }
                    else throw new Exception("没有记录");
                }
            }
            catch (Exception e)
            {
                res.status = 1;
                res.result = e.Message;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}