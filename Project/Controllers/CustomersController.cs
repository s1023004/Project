using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class CustomersController : Controller
    {
        private NorthwindDB _dbContext;
        public CustomersController(NorthwindDB _dbContext)        
        {
            this._dbContext = _dbContext;
            Console.WriteLine("注入的DbContext:" + this._dbContext.Database);
        }
        public IActionResult customersAdd(Models.Customers customers)
        {
            HttpRequest request = this.Request;
            String message = null;
            if(request.Method.Equals("POST"))
            {
                _dbContext.Customers.Add(customers);
                try
                {
                    Int32 affect = _dbContext.SaveChanges();
                    message = $"客戶編號:{customers.customerId} 新增成功!!!";
                }catch(DbUpdateException ex)
                {
                    message = $"客戶編號:{customers.customerId} 新增失敗!!!";
                }
                
            }
            Console.WriteLine(customers.companyName);
            dynamic bag = this.ViewBag;
            bag.Message = message;
            return View(); 
        }

        public IActionResult qryByCountry(String country)
        {
            HttpRequest request = this.Request;
            List<Models.Customers> customers = null;
            if(request.Method.Equals("POST")) 
            {
                var result = (from c in _dbContext.Customers
                              where c.country != null
                              select new { country = c.country.Trim() })
                             .ToList()
                             .Distinct();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var item in result)
                {
                    items.Add(
                        new SelectListItem(item.country, item.country)
                        );
                }
                //進入狀態管理 ViewData Dictionary
                this.ViewData["items"] = items;

                customers = (from c in _dbContext.Customers
                             where c.country == country
                             select c).ToList<Models.Customers>();

            }
            else
            {
                var result = (from c in _dbContext.Customers
                              where c.country != null
                             select new {country = c.country.Trim()})
                             .ToList()
                             .Distinct();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach(var item in result)
                {
                    items.Add(
                        new SelectListItem(item.country, item.country)
                        );
                }
                //進入狀態管理 ViewData Dictionary
                this.ViewData["items"] = items;
            }
            return View(customers);
        }

        public IActionResult customersInsert(Models.Customers customers)
        {
            customers.country = "中華民國";
            String message = "";
            if (this.Request.Method.Equals("POST"))
            {
                _dbContext.Customers.Add(customers);
                EntityState state = _dbContext.Entry(customers).State;
                Console.WriteLine(state.ToString());
                try
                {
                    Int32 affect = _dbContext.SaveChanges();
                    message = $"客戶編號:{customers.customerId} 新增成功了";
                }
                catch(DbUpdateException ex)
                {
                    message = $"客戶編號:{customers.customerId} 新增失敗了";
                }

            }
            ViewBag.Message = message;
            return View(customers);
        }

        //客戶查詢依照國家別 採用SPA提供UX 進行瀏覽器編輯與刪除作業
        public IActionResult customersQryByCountry(String country)
        {
            //判斷是第一次來 或者是Postback(就要查詢客戶)
            List<SelectListItem> items = null;
            if(this.Request.Method.Equals("POST"))
            {
                //透過Session取出國家別清單狀態
                String json = this.HttpContext.Session.GetString("items");
                //反序列化成List<SelectListItem>
                items = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<SelectListItem>>(json);
                var result = (from c in _dbContext.Customers
                              where c.country == country
                              select c).ToList();
                ViewBag.Items = items;
                return View(result);
            }
            else
            {
                var result = (from c in _dbContext.Customers
                              where c.country != null
                              select new { country = c.country.Trim() })
                             .ToList()
                             .Distinct();
                items = new List<SelectListItem>();
                foreach (var item in result)
                {
                    items.Add(
                        new SelectListItem(item.country, item.country)
                        );
                }
            }
            //狀態管理 同時去問出配合前端瀏覽器 Session管理(物件)
            ISession session = this.HttpContext.Session;
            //.net core跨平台 無法順利使用記憶體指標去參考物件??
            //TODO 可以將List物件序列化成Json字串(使用newton軟體第三方工具)
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            //指向Session Data持續狀態
            session.SetString("items", jsonString);
            ViewBag.Items = items; //將國家別集合物件持續狀態到View Page(Request生命週期)
            return View();
        }

        //設計傳遞客戶資料 進行資料修改作業(採用RESTful軟體風格 設定傳送方法支援Http Request Method-PUT)        
        //限制前端傳遞的資料格式 Request Header-Content-Type:application/json
        [HttpPutAttribute]
        [ConsumesAttribute("application/json")]
        public IActionResult customersUpdate([FromBodyAttribute]Models.Customers customers)
        {
            //進行查詢 在DbConext來源的物件 將傳遞進來的物件更新到來源物件屬性
            Message msg = new Message();
            //只是將Entity物件加入在前端應用系統的Persistence
            _dbContext.Customers.Add(customers);//具有新增狀態
            _dbContext.Entry(customers).State = EntityState.Modified;
            try
            {
                Int32 counter = _dbContext.SaveChanges();
                msg.code = 200;//自訂回應狀態碼
                msg.message = $"客戶:{customers.customerId} 資料更新完成";
            }
            catch (DbUpdateException)
            {
                //Http Response物件貸出的Http Status code
                //修改Http Status Code狀態碼
                this.Response.StatusCode = 400;
                //異動失敗
                msg.code = 400;//自訂回應狀態碼
                msg.message = $"客戶:{customers.customerId} 資料更新失敗";
            }
            
            return this.Json(msg);

        }

        //TODO針對傳遞客戶編號進行相對客戶資料刪除
        //自訂路由配置(採用動態路由-參數架構)
        //進行刪除作業RESTful Request Method-DELETE
        [RouteAttribute("/customers/delete/cid/{custid}/rawdata")]
        [HttpDeleteAttribute]
        [ProducesAttribute("application/json")]
        public IActionResult cusstomersDelete([FromRouteAttribute
            (Name ="custid")]String customerId)
        {
            //刪除作業 使用ado.net Entity Framework
            //先進行查詢 產生一個Persistence下的Entity物件 再將這一個Entity物件設定state 為刪除狀態
            var result = (from c in _dbContext.Customers
                          where c.customerId == customerId
                          select c).FirstOrDefault();
            //判斷是否有找到
            Message msg = new Message();
            if(result != null)
            {
                //修訂狀態碼
                _dbContext.Entry(result).State = 
                    EntityState.Deleted;                
                try
                {
                    //同步更新回資料庫
                    Int32 row = _dbContext.SaveChanges();
                    //訊息 刪除成功
                    msg.code = 200; //對應Http Status code
                    msg.message = $"客戶:{customerId} 刪除成功";
                }catch(DbUpdateConcurrencyException ex)
                {
                    //要客製化Http Status Code = 400???
                    HttpResponse response = this.Response;
                    //設定Http Status code
                    response.StatusCode = 400;
                    //例外訊息
                    msg.code = 400; //bad request
                    msg.message = $"客戶:{customerId} 刪除失敗 關聯性限制 (有訂單資料)";
                }
                catch (DbUpdateException ex)
                {
                    //要客製化Http Status Code = 400???
                    HttpResponse response = this.Response;
                    //設定Http Status code
                    response.StatusCode = 400;
                    //例外訊息
                    msg.code = 400; //bad request
                    msg.message = $"客戶:{customerId} 刪除失敗 關聯性限制 (有訂單資料)";
                }
            }
            else
            {
                //找不到那一個客戶
                msg.code = 200;
                msg.message = $"客戶:{customerId} 不存在!!!";
            }
            return Json(msg);
        }
    }
}
 