using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ByDSync.Schema;

namespace ByDSync.Controllers
{
    public class ReportGrandController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: ReportGrand
        public ActionResult Index()
        {
            ViewData["ClosingStep"] = db.enum_container.Where(x => x.enumTypeId == "CLSSTPGL").ToList();

            return View(db.ReportGLs.ToList());
        }

        // GET: ReportGrand/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportGL reportGL = db.ReportGLs.Find(id);
            if (reportGL == null)
            {
                return HttpNotFound();
            }
            return View(reportGL);
        }

        // GET: ReportGrand/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportGrand/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,Posting_Date,Journal_ID,GL_Account,Amount,Dr_Cr,Profit_Center,Project_ID,Custom_Code_3,Funding_Source_1,GL_Account_Name,Account_Type,Department_No_,Department_Name,Project_Name,Supplier_Name,Document_Source_Type,Description")] ReportGL reportGL)
        {
            if (ModelState.IsValid)
            {
                db.ReportGLs.Add(reportGL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reportGL);
        }
        
        [HttpPost]
        public ActionResult Filter()
        {
            ViewBag.StepClosing = db.enum_container.Where(x => x.enumTypeId == "CLSSTPGL").ToList();

            DateTime fromDate = Request["begin"].ToString() != "" ? DateTime.Parse(Request["begin"].ToString()).Date : DateTime.MinValue.Date;
            DateTime todate = Request["end"].ToString() != "" ? DateTime.Parse(Request["end"].ToString()).Date : DateTime.MaxValue.Date;
            string closingStep = Request["closingStep"];

            ViewBag.closingStep = closingStep;
            ViewBag.fromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.todate = todate.ToString("yyyy-MM-dd");

            var modelFilter = db.ReportGLs.Where(x => x.Posting_Date >= fromDate && x.Posting_Date <= todate && x.Closing_Step_Code == closingStep).ToList();

            return View(modelFilter);
        }

        // GET: ReportGrand/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportGL reportGL = db.ReportGLs.Find(id);
            if (reportGL == null)
            {
                return HttpNotFound();
            }
            return View(reportGL);
        }

        // POST: ReportGrand/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,Posting_Date,Journal_ID,GL_Account,Amount,Dr_Cr,Profit_Center,Project_ID,Custom_Code_3,Funding_Source_1,GL_Account_Name,Account_Type,Department_No_,Department_Name,Project_Name,Supplier_Name,Document_Source_Type,Description")] ReportGL reportGL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportGL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reportGL);
        }

        // GET: ReportGrand/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportGL reportGL = db.ReportGLs.Find(id);
            if (reportGL == null)
            {
                return HttpNotFound();
            }
            return View(reportGL);
        }

        // POST: ReportGrand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ReportGL reportGL = db.ReportGLs.Find(id);
            db.ReportGLs.Remove(reportGL);
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
