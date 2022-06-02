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
    public class ProjectTaskController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: ProjectTask
        public ActionResult Index()
        {
            return View(db.project_task.ToList());
        }

        // GET: ProjectTask/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_task project_task = db.project_task.Find(id);
            if (project_task == null)
            {
                return HttpNotFound();
            }
            return View(project_task);
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["ProjectTask"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["ProjectTask"]);
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

                List<project_task> tasks = new List<project_task>();

                project_task task;
                bool statParse;
                DateTime date;
                string tanggal;

                foreach (JObject data in jsonEnumerableData)
                {
                    task = new project_task();

                    tanggal = data.Value<JToken>("C_LastChangeDateTime").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        task.changed_on = date;
                    }

                    tanggal = data.Value<JToken>("C_CreationDateTime").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        task.created_on = date;
                    }

                    task.C_uid = data.Value<JToken>("C_TaskUuid").ToString();
                    task.project_id = data.Value<JToken>("C_ProjectId").ToString();
                    task.project_uid = data.Value<JToken>("C_ProjectUuid").ToString();
                    task.proj_resp_unit = data.Value<JToken>("C_RespCcUuid").ToString();
                    task.task_id = data.Value<JToken>("C_TaskId").ToString();
                    task.task_resp_unit = data.Value<JToken>("C_TRespCcUuid").ToString();

                    tasks.Add(task);
                }

                db.project_task.AddRange(tasks);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: ProjectTask/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,created_on,changed_on,task_id,project_uid,project_id,task_resp_unit,proj_resp_unit")] project_task project_task)
        {
            if (ModelState.IsValid)
            {
                db.project_task.Add(project_task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project_task);
        }

        // GET: ProjectTask/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_task project_task = db.project_task.Find(id);
            if (project_task == null)
            {
                return HttpNotFound();
            }
            return View(project_task);
        }

        // POST: ProjectTask/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,created_on,changed_on,task_id,project_uid,project_id,task_resp_unit,proj_resp_unit")] project_task project_task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project_task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project_task);
        }

        // GET: ProjectTask/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project_task project_task = db.project_task.Find(id);
            if (project_task == null)
            {
                return HttpNotFound();
            }
            return View(project_task);
        }

        // POST: ProjectTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            project_task project_task = db.project_task.Find(id);
            db.project_task.Remove(project_task);
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
