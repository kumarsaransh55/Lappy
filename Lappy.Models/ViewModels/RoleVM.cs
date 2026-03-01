using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lappy.Models.ViewModels
{
    public class RoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public IEnumerable<SelectListItem> CompanyList {  get; set; }
        public int ?CompanyId { get; set; }
        public string Role {  get; set; }

    }
}
