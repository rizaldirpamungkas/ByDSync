using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ByDSync.Schema;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;

namespace ByDSync.Controllers
{
    public class glModel {

        [DisplayName("Post Date")]
        public DateTime postDate { get; set; }
        [DisplayName("Journal Entry ID")]
        public string journalId { get; set; }
        [DisplayName("G/L Account")]
        public string glAccount { get; set; }
        [DisplayName("G/L Account Name")]
        public string glAccountName { get; set; }
        [DisplayName("G/L Account Type")]
        public string glAccountType { get; set; }
        [DisplayName("Description")]
        public string glDescription { get; set; }
        [DisplayName("Amount")]
        public decimal amount { get; set; }
        [DisplayName("Dept No.")]
        public string deptNo { get; set; }
        [DisplayName("Dept Name")]
        public string deptName { get; set; }
        [DisplayName("Project ID")]
        public string projectID { get; set; }
        [DisplayName("Project Name")]
        public string projectName { get; set; }
        [DisplayName("Customers/Vendors Name")]
        public string vendorName { get; set; }
        [DisplayName("Funding Source")]
        public string fundingSource { get; set; }
    }

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

        public List<glModel> reportGLs()
        {
            List<glModel> reportGLs = new List<glModel>();

            reportGLs = (from x in db.ReportGLs select new glModel {
                postDate = ((DateTime)x.Posting_Date),
                journalId = x.Journal_ID,
                glAccount = x.GL_Account,
                glAccountName = x.GL_Account_Name,
                glAccountType = x.Account_Type,
                glDescription = x.Description,
                amount = (decimal)x.Amount,
                deptNo = x.Department_No_,
                deptName = x.Department_Name,
                projectID = x.Project_ID,
                projectName = x.Project_Name,
                vendorName = x.Supplier_Name,
                fundingSource = x.Funding_Source_1
            }).ToList();

            return reportGLs; 
        }

        public void Report()
        {
            GridView gv = new GridView();

            gv.DataSource = reportGLs();
            gv.DataBind();

            string fileName = DateTime.Now.ToString("dd-MM-yyyy") + "-JCLEC-GL-Report.xls";

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename="+fileName);

            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
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
