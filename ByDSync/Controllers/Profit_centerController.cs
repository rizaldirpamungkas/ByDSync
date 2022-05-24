using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ByDSync.Miscs;
using ByDSync.Schema;
using Newtonsoft.Json.Linq;

namespace ByDSync.Controllers
{
    public class Profit_centerController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Profit_center
        public ActionResult Index()
        {
            return View(db.profit_center.ToList());
        }

        // GET: Profit_center/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            profit_center profit_center = db.profit_center.Find(id);
            if (profit_center == null)
            {
                return HttpNotFound();
            }
            return View(profit_center);
        }

        // GET: Profit_center/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["ProfitCenter"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["ProfitCenter"]);
                responseTask.Wait();

                var result = responseTask.Result;

                if (!result.IsSuccessStatusCode)
                {
                    ViewBag.JSON = "404";
                    return View();
                }

                var message = result.Content.ReadAsStringAsync();
                message.Wait();

                string jsonRaw = message.Result;
                ViewBag.JSON = jsonRaw;
                JArray jsonEnumerableData = utils.parseJson(jsonRaw);

                List<profit_center> profits = new List<profit_center>();

                profit_center profit;
                bool statParse, reason;
                int index = 0;
                string id = "PC-";

                foreach (JObject data in jsonEnumerableData)
                {
                    profit = new profit_center();

                    statParse = bool.TryParse(data.Value<JToken>("C_ForPlan").ToString(), out reason);

                    if (statParse)
                        profit.for_plan = Convert.ToInt16(reason);

                    statParse = bool.TryParse(data.Value<JToken>("C_ForPost").ToString(), out reason);

                    if (statParse)
                        profit.for_post = Convert.ToInt16(reason);

                    profit.manager_id = data.Value<JToken>("C_MgrEe").ToString();
                    profit.manager_pos = data.Value<JToken>("C_ManPos").ToString();
                    profit.org_center_id = data.Value<JToken>("C_ExtensionOrgId").ToString();
                    profit.profit_center_id = data.Value<JToken>("C_Prftctrid").ToString();
                    profit.profit_center_name = data.Value<JToken>("T_Name").ToString();
                    profit.profit_center_uuid = data.Value<JToken>("C_Prftctr").ToString();

                    profit.C_uid = id + index.ToString("D8");

                    profits.Add(profit);

                    index++;
                }

                db.profit_center.AddRange(profits);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // POST: Profit_center/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,profit_center_uuid,profit_center_id,profit_center_name,org_center_id,for_plan,for_post,manager_pos,manager_id")] profit_center profit_center)
        {
            if (ModelState.IsValid)
            {
                db.profit_center.Add(profit_center);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profit_center);
        }

        // GET: Profit_center/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            profit_center profit_center = db.profit_center.Find(id);
            if (profit_center == null)
            {
                return HttpNotFound();
            }
            return View(profit_center);
        }

        // POST: Profit_center/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,profit_center_uuid,profit_center_id,profit_center_name,org_center_id,for_plan,for_post,manager_pos,manager_id")] profit_center profit_center)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profit_center).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profit_center);
        }

        // GET: Profit_center/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            profit_center profit_center = db.profit_center.Find(id);
            if (profit_center == null)
            {
                return HttpNotFound();
            }
            return View(profit_center);
        }

        // POST: Profit_center/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            profit_center profit_center = db.profit_center.Find(id);
            db.profit_center.Remove(profit_center);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
