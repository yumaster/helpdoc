using Dapper;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UNIT;

namespace webApp.Controllers
{
    //[AuthFilter]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加目录
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> AddCatalog(string name, int pid = 0)
        {
            httpResult res = new httpResult();
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new Exception("目录名称不能为空");
                }
                else
                {
                    using (IDbConnection db = dbUtil.createDB())
                    {
                        StringBuilder sb = new StringBuilder("declare @node HierarchyID,@pnode HierarchyID;");
                        if (pid > 0)
                        {
                            sb.Append("select @pnode=node from mulu where id=@pid;select @node=@pnode.GetDescendant(MAX(node),null) from mulu where node.GetAncestor(1)=@pnode;");
                        }
                        else
                        {
                            sb.Append("select @node=HierarchyID::Parse('/').GetDescendant(MAX(node),null) from mulu where node.GetLevel()=1;");
                        }
                        sb.Append("insert into mulu(pid,node,name)values(@pid,@node,@name);SELECT @@IDENTITY");
                        int id = await db.QuerySingleOrDefaultAsync<int>(sb.ToString(), new { pid, name });
                        if (id > 0)
                        {
                            await RemoveCache();//清空缓存
                            res.result = new Catalog
                            {
                                id = id,
                                n = name,
                                i = false
                            };
                        }
                        else throw new Exception("添加失败");
                    }
                }
            }
            catch (Exception e)
            {
                res.status = 1;
                res.result = e.Message;
            }
            return Json(res);
        }
        /// <summary>
        /// 重命名
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ReName(int id, string name)
        {
            httpResult res = new httpResult();
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new Exception("目录名称不能为空");
                }
                else
                {
                    using (IDbConnection db = dbUtil.createDB())
                    {
                        
                        int row= await db.ExecuteAsync("update mulu set name=@name where id=@id", new { id, name });
                        if (row > 0)
                        {
                            
                            res.result = "修改成功";
                        }
                        else throw new Exception("修改失败");
                    }
                }
            }
            catch (Exception e)
            {
                res.status = 1;
                res.result = e.Message;
            }
            return Json(res);
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DelCatalog(int id)
        {
            httpResult res = new httpResult();
            try
            {
                using (IDbConnection db = dbUtil.createDB())
                {
                    string sql = @"declare @node hierarchyid;select @node=node from mulu where id=@id
update mulu set isdel=1 where node.IsDescendantOf(@node)=1 and isdel=0";
                    if (await db.ExecuteAsync(sql, new { id }) > 0)
                    {
                        await RemoveCache();//清空缓存
                        res.result = "删除成功";
                    }
                    else throw new Exception("删除失败");
                }
            }
            catch (Exception e)
            {
                res.status = 1;
                res.result = e.Message;
            }
            return Json(res);
        }
        
        /// <summary>
        /// 获得树
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> getTree(int id=0)
        {
            httpResult res = new httpResult();
            try
            {
                //string key = "HELPTREE",
                //       dkey= id.ToString();
                //RedisCache cache = new RedisCache(1);
                //if (await cache.Do(cdb => cdb.HashExistsAsync(key, dkey))) {
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
                //        if (list.Count>0)
                //        {
                //            await cache.Do(cdb => cdb.HashSetAsync(key,dkey,JsonConvert.SerializeObject(list)));
                //            await cache.Do(cdb => cdb.KeyExpireAsync(key,TimeSpan.FromDays(88)));
                //            res.result = list;
                //        }
                //        else throw new Exception("没有记录");
                //    }
                //}
                using (IDbConnection db = dbUtil.createDB())
                {
                    string sql = "select id,name[n],url[u],convert(bit,case when b.pid is not null then 1 else 0 end) as i from mulu a left join(select pid from mulu group by pid) b on a.id=b.pid where a.pid=@id and a.isdel=0 order by node";
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
        /// <summary>
        /// 保存内容
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> UeditorForm(int id,string content) {
            httpResult res = new httpResult();
            try
            {
                if (string.IsNullOrWhiteSpace(content)) throw new Exception("没有任何内容");
                else
                {
                    string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".html",
                           filePaht= $"/upl/html/{DateTime.Now.ToString("MMyyyydd")}",
                           dirPath = Server.MapPath(filePaht);
                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);//如果目录不存在就创建
                    System.IO.File.WriteAllText($"{dirPath}\\{fileName}", content);
                    string url = $"{filePaht.Replace(@"\", "/")}/{fileName}";
                    using (IDbConnection db = dbUtil.createDB())
                    {
                        string sql = "UPDATE mulu SET url=@url WHERE id=@id and isdel=0;INSERT INTO log_mulu(mid,url)VALUES(@id,@url)";
                        if (await db.ExecuteAsync(sql,new { id,url}) > 0)
                        {
                            await RemoveCache();//清空缓存
                            res.result = url;
                        }
                        else throw new Exception("保存失败");
                    }
                }
            }
            catch (Exception e)
            {
                res.status = 1;
                res.result = e.Message;
            }
            return Json(res);
        }

        public async Task<ActionResult> __getContent(int id) {
            using (IDbConnection db = dbUtil.createDB())
            {
                string url = await db.QuerySingleOrDefaultAsync<string>("select top(1)url from mulu where isdel=0 and id=@id", new { id });
                if (string.IsNullOrWhiteSpace(url))return Content("无内容");
                else
                {
                    string path = Server.MapPath(url);
                    if (System.IO.File.Exists(path))
                    {
                        return Content(System.IO.File.ReadAllText(path), "text/html");
                    }
                    else return Content("文件不存在");
                }
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        private async Task<bool> RemoveCache() {
            return await new RedisCache(1).Do(cdb => cdb.KeyDeleteAsync("HELPTREE"));
        }
    }
}