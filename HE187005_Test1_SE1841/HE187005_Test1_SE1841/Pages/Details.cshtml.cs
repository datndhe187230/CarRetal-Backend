using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HE187005_Test1_SE1841.Data;
using HE187005_Test1_SE1841.Model;

namespace HE187005_Test1_SE1841.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly HE187005_Test1_SE1841.Data.AppDbContext _context;

        public DetailsModel(HE187005_Test1_SE1841.Data.AppDbContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmpID == id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                Employee = employee;
            }
            return Page();
        }
    }
}
