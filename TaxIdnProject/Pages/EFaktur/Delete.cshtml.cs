using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaxIdnProject.Data;
using TaxIdnProject.Models;

namespace TaxIdnProject.Pages.EFaktur
{
    public class DeleteModel : PageModel
    {
        private readonly TaxIdnProject.Data.TaxIdnContext _context;

        public DeleteModel(TaxIdnProject.Data.TaxIdnContext context)
        {
            _context = context;
        }

        [BindProperty]
      public EFakturContainer EFakturContainer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.EFakturContainer == null)
            {
                return NotFound();
            }

            var efakturcontainer = await _context.EFakturContainer.FirstOrDefaultAsync(m => m.ID == id);

            if (efakturcontainer == null)
            {
                return NotFound();
            }
            else 
            {
                EFakturContainer = efakturcontainer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.EFakturContainer == null)
            {
                return NotFound();
            }
            var efakturcontainer = await _context.EFakturContainer.FindAsync(id);

            if (efakturcontainer != null)
            {
                EFakturContainer = efakturcontainer;
                _context.EFakturContainer.Remove(EFakturContainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
