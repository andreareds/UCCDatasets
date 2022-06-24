using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace UCC_Datasets.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IdentityInitController : Controller
    {
        private UserManager<IdentityUser> userManager;

        public IdentityInitController(UserManager<IdentityUser> usrMgr)
        {
            userManager = usrMgr;
        }

        public async Task<IActionResult> Creation()
        {

            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"Data//users.json");
            var jsonFile = System.IO.File.ReadAllText(folderDetails);
            dynamic recJson = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFile);

            foreach (var item in recJson["users"])
            {
                String username = item["username"];
                IdentityUser user = new IdentityUser(username);
                user.EmailConfirmed = true;
                user.Email = item["username"];
                IdentityResult result = await userManager.CreateAsync(user, (String)item["password"]);
                int a = 0;
            }


            return View();
        }
    }
}