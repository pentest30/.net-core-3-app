using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Voluntary.App.Data.Entities;

namespace Voluntary.App.Data
{
    public class SeedDummyData
    {
        private readonly ApplicationDbContext context;

        public SeedDummyData(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = {"admin", "guest", "user"};

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
                if (!roleExist)
                    //create the roles and seed them to the database !!
                    await roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false);
            }
        }

        public Task CreateDistrictsAsync(IServiceProvider serviceProvider)
        {
            string[] names = { "Rue 1er novembre", 
                "Rue Ait Dahmane Hamou", 
                "Rue Belssaadi Abdelkader" ,
                "Rue Chaichi abdelkader",
                "Rue de la Gare",
                "Rue Djaadane Abdelkader",
                "Rue Djitli Mustapha",
                "Rue Ghida Benyoucef",
                "Rue Kacedali Ahmed",
                "Rue Larbi Bouamran Abdallah",
                "Rue Maaiza Abdelkader",
                "Rue Malki Sehane",
                "Rue Tahar Kouadri Mohamed"
            };
            foreach (var name in names)
            {
                var dist =  new District
                {
                    Name = name,
                    NameAr = "",
                    City = "Khemis Miliana",
                    Department = "Ain defla",
                    ZipCode = "44225"
                };
                if(context.Districts.Any(x=>x.Name == name))
                    continue;
                context.Districts.Add(dist);
            }

            return context.SaveChangesAsync();

        }
        public async Task CreateUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@volunteer.net",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "a$123456").ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var currentUser = await userManager.FindByNameAsync(user.UserName).ConfigureAwait(false);
                    await userManager.AddToRoleAsync(currentUser, "admin").ConfigureAwait(false);
                }
            }
        }
    }
}
