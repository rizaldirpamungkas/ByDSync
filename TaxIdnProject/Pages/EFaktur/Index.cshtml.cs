using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaxIdnProject.Data;
using TaxIdnProject.Models;

namespace TaxIdnProject.Pages.EFaktur
{
    public class IndexModel : PageModel
    {
        private readonly TaxIdnContext _context;

        public IndexModel(TaxIdnContext context, ILogger<IndexModel> logger)
        {
            _context = context;
        }

        public IList<EFakturContainer> EFakturContainer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.EFakturContainer != null)
            {
                EFakturContainer = await _context.EFakturContainer.Where(a => a.isMasukan == true).ToListAsync();
            }
        }

        public ActionResult OnGetData()
        {
            ByDSync byDSync = new ByDSync(_context);

            //byDSync.populateEfakturIn();

            return LocalRedirect("/Efaktur");
        }

        public ActionResult OnPostData()
        {
            ByDSync byDSync = new ByDSync(_context);

            DateOnly tanggalDari = Request.Form["dariTanggal"] != "" ? DateOnly.Parse(Request.Form["dariTanggal"]) : DateOnly.MinValue;
            DateOnly tanggalSampai = Request.Form["sampaiTanggal"] != "" ? DateOnly.Parse(Request.Form["sampaiTanggal"]) : DateOnly.MaxValue;
            string kodeSupplier = Request.Form["kodeSupplier"];


            if(tanggalDari == DateOnly.MinValue)
            {
                ViewData["WarningMessage"] = "Tanggal Dari Harap Diisi untuk menghindari overload data!";
                return null;
            }

            if (tanggalSampai == DateOnly.MaxValue)
            {
                ViewData["WarningMessage"] = "Tanggal Sampai Harap Diisi untuk menghindari overload data!";
                return null;
            }

            byDSync.populateEfakturIn(tanggalDari, tanggalSampai, kodeSupplier);

            return LocalRedirect("/Efaktur");
        }
        public IActionResult OnGetCSV()
        {
            ByDSync byDSync = new ByDSync(_context);

            MemoryStream stream = new MemoryStream();
            
            byDSync.generateCSV(stream, false);
            stream.Position = 0;

            return File(stream, "application/octet-stream", "EFakturIn.csv");
        }
    }
}
