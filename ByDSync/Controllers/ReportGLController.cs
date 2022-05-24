using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ByDSync.Schema;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;

namespace ByDSync.Controllers
{
    public class ReportGLController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: ReportGL
        public ActionResult Index()
        {
            return View(db.ReportGLs.ToList());
        }

        // GET: ReportGL/Details/5
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

        // GET: ReportGL/Create
        public ActionResult Create()
        {
            return View();
        }
        
        public void Report()
        {
            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(Server.MapPath("Reports/GenLedger.rpt"));
            CrystalReportViewer reportViewer = new CrystalReportViewer();

            reportViewer.ReportSource = cryRpt;

            //return View();
        }

        // POST: ReportGL/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,Posting_Date,Journal_ID,GL_Account,Amount,Dr_Cr,Profit_Center,Project_ID,Custom_Code_3,Funding_Source_1")] ReportGL reportGL)
        {
            if (ModelState.IsValid)
            {
                db.ReportGLs.Add(reportGL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reportGL);
        }

        // GET: ReportGL/Edit/5
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

        // POST: ReportGL/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,Posting_Date,Journal_ID,GL_Account,Amount,Dr_Cr,Profit_Center,Project_ID,Custom_Code_3,Funding_Source_1")] ReportGL reportGL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportGL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reportGL);
        }

        // GET: ReportGL/Delete/5
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

        // POST: ReportGL/Delete/5
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
