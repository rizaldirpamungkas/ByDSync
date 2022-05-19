using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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
    public class ProjectsController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["Project"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["Project"]);
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

                List<project> projects = new List<project>();

                project project;
                decimal num;
                bool statParse, onHold;
                DateTime date;
                int index = 0;
                string id = "PJ-";

                foreach (JObject data in jsonEnumerableData)
                {
                    project = new project();

                    string costCenterRaw = data.Value<JToken>("to_RespCcUuid").ToString();
                    JObject toCostCenter = costCenterRaw != "" ? JObject.Parse(costCenterRaw) : null;
                    
                    string costCenter = toCostCenter != null ? toCostCenter.Value<JToken>("C_Costctr").ToString() : "";
                    string costCenterId = toCostCenter != null ? toCostCenter.Value<JToken>("C_Costctrid").ToString() : "";
                    string costCenterType = toCostCenter != null ? toCostCenter.Value<JToken>("C_CcType").ToString() : "";
                    string profitCenter = toCostCenter != null ? toCostCenter.Value<JToken>("C_Profitctr").ToString() : "";

                    project.cost_center_id = costCenterId;
                    project.cost_center_type = costCenterType;
                    project.cost_ctr_id_respunit = costCenter;
                    project.profit_center_id = profitCenter;

                    project.company = data.Value<JToken>("C_CompanyUuid").ToString();
                    project.customer = data.Value<JToken>("C_BuyerPtyUuid").ToString();
                    project.project_id = data.Value<JToken>("C_ProjectId").ToString();
                    project.project_type = data.Value<JToken>("C_ProjType").ToString();
                    project.project_uuid = data.Value<JToken>("C_ProjectUuid").ToString();
                    project.proj_desc = data.Value<JToken>("T_Description").ToString();
                    project.risk_level = data.Value<JToken>("C_RiskLevel").ToString();
                    project.status = data.Value<JToken>("C_StatusLfc").ToString();

                    statParse = bool.TryParse(data.Value<JToken>("C_StatusBlocking").ToString(), out onHold);

                    if(statParse)
                        project.onhold = Convert.ToInt16(onHold);

                    statParse = DateTime.TryParse(data.Value<JToken>("C_ProjAEndDat").ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                    if (statParse)
                        project.projActEndDate = date;

                    statParse = DateTime.TryParse(data.Value<JToken>("C_ProjAStDat").ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                    if (statParse)
                        project.projActStaDate = date;

                    project.C_uid = id + index.ToString("D8");

                    projects.Add(project);

                    index++;
                }

                db.projects.AddRange(projects);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,project_id,project_uuid,project_type,projActStaDate,projActEndDate,proj_desc,profit_center_id,cost_ctr_id_respunit,cost_center_type,cost_center_id,customer,company,status,risk_level,onhold")] project project)
        {
            if (ModelState.IsValid)
            {
                db.projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,project_id,project_uuid,project_type,projActStaDate,projActEndDate,proj_desc,profit_center_id,cost_ctr_id_respunit,cost_center_type,cost_center_id,customer,company,status,risk_level,onhold")] project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            project project = db.projects.Find(id);
            db.projects.Remove(project);
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
