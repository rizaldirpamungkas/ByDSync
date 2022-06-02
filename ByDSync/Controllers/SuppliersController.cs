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
    public class SuppliersController : Controller
    {
        private ByDMirrorEntities db = new ByDMirrorEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
            return View(db.suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            supplier supplier = db.suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Synchronize()
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + param.tableName["Supplier"]);

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityAPI["Username"] + ":" + param.securityAPI["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                var responseTask = client.GetAsync(param.uriAPI["Supplier"]);
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

                List<supplier> suppliers = new List<supplier>();

                supplier supplier;
                bool statParse;
                DateTime date;
                string tanggal;

                foreach (JObject data in jsonEnumerableData)
                {
                    supplier = new supplier();

                    tanggal = data.Value<JToken>("C_BpCrnDatetime").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        supplier.changed_on = date;
                    }

                    tanggal = data.Value<JToken>("C_BpLastChgDt").ToString();

                    if (tanggal != "")
                    {
                        date = DateTime.Parse(tanggal);
                        supplier.created_on = date;
                    }

                    supplier.C_uid = data.Value<JToken>("C_BpUuid").ToString();
                    supplier.supplier_adr = data.Value<JToken>("C_FrmtdPstlAddr").ToString() + " " + data.Value<JToken>("C_AddrStPrefix").ToString() + " " + data.Value<JToken>("C_AddrAddStreetPrefix").ToString() + " " + data.Value<JToken>("C_AddrStPrefix").ToString() + " " + data.Value<JToken>("C_AddrStSuffix").ToString();
                    supplier.supplier_city = data.Value<JToken>("C_CityName").ToString();
                    supplier.supplier_country = data.Value<JToken>("C_CntryCode").ToString();
                    supplier.supplier_email = data.Value<JToken>("C_EmailUri").ToString();
                    supplier.supplier_id = data.Value<JToken>("C_BpIntId").ToString();
                    supplier.supplier_ind_code = data.Value<JToken>("C_IndssctrCode").ToString();
                    supplier.supplier_ind_sys = data.Value<JToken>("C_Indssystem").ToString();
                    supplier.supplier_name = data.Value<JToken>("T_BpFrmtdName").ToString();
                    supplier.supplier_phone = data.Value<JToken>("C_AddrFrmtdPh").ToString();
                    supplier.supplier_pobox = data.Value<JToken>("C_PoboxId").ToString(); ;
                    supplier.supplier_postcode = data.Value<JToken>("C_PoboxId").ToString();
                    supplier.supplier_state = data.Value<JToken>("C_RegionCode").ToString();
                    supplier.supplier_street = data.Value<JToken>("C_StName").ToString();

                    suppliers.Add(supplier);
                }

                db.suppliers.AddRange(suppliers);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_uid,created_on,changed_on,supplier_id,supplier_name,supplier_email,supplier_adr,supplier_pobox,supplier_postcode,supplier_street,supplier_city,supplier_state,supplier_country,supplier_ind_code,supplier_ind_sys")] supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            supplier supplier = db.suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_uid,created_on,changed_on,supplier_id,supplier_name,supplier_email,supplier_adr,supplier_pobox,supplier_postcode,supplier_street,supplier_city,supplier_state,supplier_country,supplier_ind_code,supplier_ind_sys")] supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            supplier supplier = db.suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            supplier supplier = db.suppliers.Find(id);
            db.suppliers.Remove(supplier);
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
