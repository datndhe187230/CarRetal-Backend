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
    public class IndexModel : PageModel
    {
        private readonly HE187005_Test1_SE1841.Data.AppDbContext _context;

        public IndexModel(HE187005_Test1_SE1841.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set;}

        public async Task OnGetAsync()
        {
            var employees = from e in _context.Employees
                            select e;

            if (!string.IsNullOrEmpty(SearchString))
            {
                employees = employees.Where(e => e.EmpName.Contains(SearchString));
            }

            Employee = await employees.ToListAsync();
        }
    }
}
