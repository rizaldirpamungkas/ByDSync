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
    public class Gen_ledgerController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Gen_ledger
        public ActionResult Index()
        {
            return View(db.gen_ledger.ToList());
        }

        // GET: Gen_ledger/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            gen_ledger gen_ledger = db.gen_ledger.Find(id);
            if (gen_ledger == null)
            {
                return HttpNotFound();
            }
            return View(gen_ledger);
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["GeneralLedger"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["GeneralLedger"]);
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

                List<gen_ledger> gens = new List<gen_ledger>();

                gen_ledger gen;
                decimal num;
                bool statParse;
                DateTime date;
                int index = 0;
                string tanggal;
                string id = "GL-";

                foreach (JObject data in jsonEnumerableData)
                {
                    gen = new gen_ledger();

                    tanggal = data.Value<JToken>("C_CreationDate").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        gen.created_date = date;
                    }

                    tanggal = data.Value<JToken>("C_LaChangeDate").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        gen.changed_date = date;
                    }

                    gen.jour_id = data.Value<JToken>("C_AccDocUuid").ToString();
                    gen.jour_item_id = data.Value<JToken>("C_AccDocItUuid").ToString();
                    gen.jour_type = data.Value<JToken>("C_Accdoctype").ToString();
                    gen.bus_part_addr = data.Value<JToken>("C_AddrBusPart").ToString();
                    gen.chart_of_act = data.Value<JToken>("C_Chofaccts").ToString();
                    gen.cus_code1 = data.Value<JToken>("C_CustomCode1").ToString();
                    gen.cus_code2 = data.Value<JToken>("C_CustomCode2").ToString();
                    gen.cus_code3 = data.Value<JToken>("C_CustomCode3").ToString();
                    gen.debit_credit = int.Parse(data.Value<JToken>("C_Debitcredit").ToString());
                    gen.employee_id = data.Value<JToken>("C_EmployeeUuid").ToString();
                    gen.fiscal_year = int.Parse(data.Value<JToken>("C_Fiscyear").ToString());
                    gen.gl_acct = data.Value<JToken>("C_Glacct").ToString();
                    gen.gl_acct_type = data.Value<JToken>("C_GlacctTc").ToString();
                    
                    string profitCenterRaw = data.Value<JToken>("to_ProfitctrUuid").ToString();
                    JObject toProfitCenter = profitCenterRaw != "" ? JObject.Parse(profitCenterRaw) : null;
                    string profitCenter = toProfitCenter != null ? toProfitCenter.Value<JToken>("C_Prftctrid").ToString() : "";

                    gen.profit_center_id = profitCenter;

                    string projectIdRaw = data.Value<JToken>("to_ProjTaskUuid").ToString();
                    JObject toProjTask = projectIdRaw != ""  ? JObject.Parse(projectIdRaw) : null;
                    string projectId = toProjTask != null ? toProjTask.Value<JToken>("C_ProjectId").ToString() : "";

                    gen.project_id = projectId;

                    tanggal = data.Value<JToken>("C_PostingDate").ToString(); ;

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        gen.post_date = date;
                    }

                    int div = 1000000;

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amtcomp").ToString(), out num);

                    if (statParse)
                        gen.comp_cur_amt = num/div;

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amtlit").ToString(), out num);

                    if (statParse)
                        gen.item_cur_amt = num/div;

                    statParse = decimal.TryParse(data.Value<JToken>("K_Amttra").ToString(), out num);

                    if (statParse)
                        gen.tran_cur_amt = num/div;

                    statParse = decimal.TryParse(data.Value<JToken>("K_ValQuantity").ToString(), out num);

                    if (statParse)
                        gen.val_qty_unt = num/div;

                    gen.C_uid = id + index.ToString("D8");

                    index++;

                    gens.Add(gen);
                }

                //ViewBag.JSON = gens[3].profit_center_id + " " + gens[5].project_id;

                db.gen_ledger.AddRange(gens);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Gen_ledger/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Gen_ledger/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,created_date,changed_date,jour_id,jour_item_id,jour_type,bus_part_addr,chart_of_act,cus_code1,cus_code2,cus_code3,debit_credit,employee_id,fiscal_year,gl_acct,gl_acct_type,project_id,profit_center_id,post_date,prod_id,comp_cur_amt,item_cur_amt,tran_cur_amt,val_qty_unt")] gen_ledger gen_ledger)
        {
            if (ModelState.IsValid)
            {
                db.gen_ledger.Add(gen_ledger);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gen_ledger);
        }

        // GET: Gen_ledger/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            gen_ledger gen_ledger = db.gen_ledger.Find(id);
            if (gen_ledger == null)
            {
                return HttpNotFound();
            }
            return View(gen_ledger);
        }

        // POST: Gen_ledger/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,created_date,changed_date,jour_id,jour_item_id,jour_type,bus_part_addr,chart_of_act,cus_code1,cus_code2,cus_code3,debit_credit,employee_id,fiscal_year,gl_acct,gl_acct_type,project_id,profit_center_id,post_date,prod_id,comp_cur_amt,item_cur_amt,tran_cur_amt,val_qty_unt")] gen_ledger gen_ledger)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gen_ledger).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gen_ledger);
        }

        // GET: Gen_ledger/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            gen_ledger gen_ledger = db.gen_ledger.Find(id);
            if (gen_ledger == null)
            {
                return HttpNotFound();
            }
            return View(gen_ledger);
        }

        // POST: Gen_ledger/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            gen_ledger gen_ledger = db.gen_ledger.Find(id);
            db.gen_ledger.Remove(gen_ledger);
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
