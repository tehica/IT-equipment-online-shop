using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace OnlineShop.Models.ViewModels
{
    public class ListRoleViewModel
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
