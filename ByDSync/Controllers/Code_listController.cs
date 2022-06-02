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
    public class Code_listController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Code_list
        public ActionResult Index()
        {
            return View(db.code_list.ToList());
        }

        // GET: Code_list/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            code_list code_list = db.code_list.Find(id);
            if (code_list == null)
            {
                return HttpNotFound();
            }
            return View(code_list);
        }

        // GET: Code_list/Create
        public ActionResult Create()
        {
            return View();
        }
        
        public ActionResult SynchronizeCC3()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("DELETE FROM " + param.tableName["CustomCode3"] + " WHERE typeCode = 'CustomCode3'");

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["CustomCode3"]);
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

                List<code_list> codes = new List<code_list>();

                code_list code;

                foreach (JObject data in jsonEnumerableData)
                {
                    code = new code_list();

                    code.code = data.Value<JToken>("Code").ToString(); ;
                    code.description = data.Value<JToken>("Description").ToString(); ;
                    code.typeCode = "CustomCode3";

                    codes.Add(code);
                }

                db.code_list.AddRange(codes);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //Add by Hdk 25-05-2022
        public ActionResult SynchronizeCC1()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            // db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["CustomCode3"]);
            db.Database.ExecuteSqlCommand("DELETE FROM" + " " + param.tableName["CustomCode1"] + " " + "where typeCode = 'CustomCode1'");

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["CustomCode1"]);
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

                List<code_list> codes = new List<code_list>();

                code_list code;

                foreach (JObject data in jsonEnumerableData)
                {
                    code = new code_list();

                    code.code = data.Value<JToken>("Code").ToString(); ;
                    code.description = data.Value<JToken>("Description").ToString(); ;
                    code.typeCode = "CustomCode1";

                    codes.Add(code);
                }

                db.code_list.AddRange(codes);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Code_list/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "code,description,typeCode")] code_list code_list)
        {
            if (ModelState.IsValid)
            {
                db.code_list.Add(code_list);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(code_list);
        }

        // GET: Code_list/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            code_list code_list = db.code_list.Find(id);
            if (code_list == null)
            {
                return HttpNotFound();
            }
            return View(code_list);
        }

        // POST: Code_list/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "code,description,typeCode")] code_list code_list)
        {
            if (ModelState.IsValid)
            {
                db.Entry(code_list).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(code_list);
        }

        // GET: Code_list/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            code_list code_list = db.code_list.Find(id);
            if (code_list == null)
            {
                return HttpNotFound();
            }
            return View(code_list);
        }

        // POST: Code_list/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            code_list code_list = db.code_list.Find(id);
            db.code_list.Remove(code_list);
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
