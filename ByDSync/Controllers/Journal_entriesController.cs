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
    public class Journal_entriesController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Journal_entries
        public ActionResult Index()
        {
            return View(db.journal_entries.ToList());
        }

        // GET: Journal_entries/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            journal_entries journal_entries = db.journal_entries.Find(id);
            if (journal_entries == null)
            {
                return HttpNotFound();
            }
            return View(journal_entries);
        }

        // GET: Journal_entries/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["JournalEntry"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["JournalEntry"]);
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

                List<journal_entries> journals = new List<journal_entries>();

                journal_entries journal;
                decimal num;
                bool statParse;
                DateTime date;
                string tanggal;
                int index = 0;
                int div = 1000000;
                string id = "JE-";

                foreach (JObject data in jsonEnumerableData)
                {
                    journal = new journal_entries();

                    tanggal = data.Value<JToken>("C_SaLcDateTime").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        journal.changed_date = date;
                    }

                    tanggal = data.Value<JToken>("C_CreationDate").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        journal.created_date = date;
                    }

                    journal.changed_by = data.Value<JToken>("C_SaLcIdUuid").ToString();
                    journal.created_by = data.Value<JToken>("C_SaCrIdUuid").ToString();
                    journal.debit_credit = int.Parse(data.Value<JToken>("C_Debitcredit").ToString());
                    journal.fiscal_year = int.Parse(data.Value<JToken>("C_Fiscyear").ToString());
                    journal.gl_acct_type = data.Value<JToken>("C_GlacctTc").ToString();
                    journal.gl_act = data.Value<JToken>("C_Glacct").ToString();
                    journal.jour_id = data.Value<JToken>("C_AccDocUuid").ToString();
                    journal.jour_number = data.Value<JToken>("C_AccDocId").ToString();

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amtcomp").ToString(), out num);

                    if (statParse)
                        journal.comp_cur_amt = num / div;

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amtlit").ToString(), out num);

                    if (statParse)
                        journal.item_cur_amt = num / div;

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amttra").ToString(), out num);

                    if (statParse)
                        journal.tran_cur_amt = num / div;

                    journal.C_uid = id + index.ToString("D8");

                    journals.Add(journal);

                    index++;
                }

                db.journal_entries.AddRange(journals);
                db.SaveChanges();
             }

            return RedirectToAction("Index");
        }

        // POST: Journal_entries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,gl_act,gl_acct_type,created_date,changed_date,posting_date,jour_id,debit_credit,fiscal_year,created_by,changed_by,comp_cur_amt,item_cur_amt,tran_cur_amt,jour_number")] journal_entries journal_entries)
        {
            if (ModelState.IsValid)
            {
                db.journal_entries.Add(journal_entries);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(journal_entries);
        }

        // GET: Journal_entries/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            journal_entries journal_entries = db.journal_entries.Find(id);
            if (journal_entries == null)
            {
                return HttpNotFound();
            }
            return View(journal_entries);
        }

        // POST: Journal_entries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,gl_act,gl_acct_type,created_date,changed_date,posting_date,jour_id,debit_credit,fiscal_year,created_by,changed_by,comp_cur_amt,item_cur_amt,tran_cur_amt,jour_number")] journal_entries journal_entries)
        {
            if (ModelState.IsValid)
            {
                db.Entry(journal_entries).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(journal_entries);
        }

        // GET: Journal_entries/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            journal_entries journal_entries = db.journal_entries.Find(id);
            if (journal_entries == null)
            {
                return HttpNotFound();
            }
            return View(journal_entries);
        }

        // POST: Journal_entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            journal_entries journal_entries = db.journal_entries.Find(id);
            db.journal_entries.Remove(journal_entries);
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
