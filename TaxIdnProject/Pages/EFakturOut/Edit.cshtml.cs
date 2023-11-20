using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaxIdnProject.Data;
using TaxIdnProject.Models;

namespace TaxIdnProject.Pages.EFakturOut
{
    public class EditModel : PageModel
    {
        private readonly TaxIdnProject.Data.TaxIdnContext _context;

        public EditModel(TaxIdnProject.Data.TaxIdnContext context)
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

            var efakturcontainer =  await _context.EFakturContainer.FirstOrDefaultAsync(m => m.ID == id);
            if (efakturcontainer == null)
            {
                return NotFound();
            }
            EFakturContainer = efakturcontainer;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(EFakturContainer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EFakturContainerExists(EFakturContainer.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EFakturContainerExists(int id)
        {
          return (_context.EFakturContainer?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
