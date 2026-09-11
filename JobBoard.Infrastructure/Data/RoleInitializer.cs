using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client.NativeInterop;

namespace JobBoard.Infrastructure.Data
{
    public class RoleInitializer
    {
        private readonly RoleManager<IdentityRole> roleManager;
         public RoleInitializer(RoleManager<IdentityRole>roleManager)
        {
            this.roleManager = roleManager;
        }
        public async Task InitializeRolesAsync()
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
             await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Employer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Employer"));
            }

            if (!await roleManager.RoleExistsAsync("JobSeeker"))
            {
                await roleManager.CreateAsync(new IdentityRole("JobSeeker"));
            }
        }
    }
}
