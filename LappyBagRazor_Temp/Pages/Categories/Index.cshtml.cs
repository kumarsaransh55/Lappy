using LappyBagRazor_Temp.Data;
using LappyBagRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LappyBagRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _db;
        public IndexModel(ApplicationDBContext db)
        {
            _db = db;
        }
        public List<Category> CategoryList { get; set; }
        public void OnGet()
        {
            CategoryList=_db.Categories.ToList();
        }
    }
}
