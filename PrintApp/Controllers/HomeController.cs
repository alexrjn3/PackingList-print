using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PrintApp.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.DirectoryServices;

namespace ScannerAp11.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Home  
        [Authorize]
        public IActionResult Index()
        {
            /* //In case Packing List is an attribute in db. We can show all of them as buttons on Index Page. Packing List 1, Packing List 2, etc
            var data = LoadInfo3();
            List<PackingListModel> info = new List<PackingListModel>();
            
            List<string> packinglist = new List<string>();
            foreach(var row in data)
            {
                packinglist.Add(row.PackingList);
            }
           
            foreach (string dif in packinglist.Distinct())
            {
                info.Add(new PackingListModel
                {
                    PackingList = dif,
                });
            }*/
            return View();
        }

        //Call DatabaseController,LoadData method to take data from DB
        public static List<PackingListModel> LoadInfo(string pal)
        {
            string pal2 = "'" + pal + "'";
            string sql = @"  SELECT TOP (10000) [Palet],[Colet],[Material],[DescriereMaterial],[Cantitate] FROM [test_WEB2].[dbo].[PrintMaterialTable] WHERE [Colet] IS NOT NULL ORDER BY [Palet] ASC";
            return DatabaseController.LoadData<PackingListModel>(sql);
        }

        public static List<LoginModel> LoadUser()
        {
            string sql = @"SELECT username,pass FROM Table_1 ORDER BY ID DESC;";
            return DatabaseController.LoadData<LoginModel>(sql);
        }


        /* not needed anymore
        public static List<PackingListModel> LoadInfo3()
        {
            string sql = @"SELECT PackingList FROM packingTable2 ORDER BY ID ASC;";
            return DatabaseController.LoadData<PackingListModel>(sql);
        }
        */

        [Authorize]
        //Page ViewList - Table
        public ActionResult ViewList(string pal)
        {
            ViewBag.TextPAL2 = pal; 
            ViewBag.TextTime = DateTime.Now;
            var data = LoadInfo(pal);
            List<PackingListModel> info = new List<PackingListModel>();


            //algo to show a Palet number once in the table view. Duplicate numbers are replaced with an empty string
            //start
            List<string> AllPalet = new List<string>();
            int stop = 0;
            AllPalet.Add(data[0].Palet);
            for (int i = 1; i < data.Count; i++)
            {
                for (int j = 0; j < AllPalet.Count; j++)
                {
                    if (data[i].Palet == AllPalet[j])
                    {
                        for (int k = AllPalet.Count - 1; k > AllPalet.Count - 2; k--)
                        {
                            if (data[i].Palet == AllPalet[k])
                            {
                                AllPalet.Add(data[i].Palet);
                                data[i].Palet = "";
                                stop = 1;
                            }
                        }
                    }
                    if (stop == 1) break;
                }
                if (stop != 1) { AllPalet.Add(data[i].Palet); stop = 0; }
                stop = 0;
            }
            //end
            
            //send data from DB to Print VIEW to show them in the table 
            foreach (var row in data)
            {


                info.Add(new PackingListModel
                {
                    Palet = row.Palet,
                    Colet = row.Colet,
                    Material = row.Material,
                    DescriereMaterial = row.DescriereMaterial,
                    Cantitate = row.Cantitate
                });
            }
            return View(info);
        }

        //Page Print. Same as ViewList
        [Authorize]
        public ActionResult Print(string pal, string orv)
        {
            ViewBag.TextPAL2 = pal;
            ViewBag.TextTime = DateTime.Now;
            List<string> AllPalet = new List<string>();
           


            var data = LoadInfo(pal);

            List<PackingListModel> info = new List<PackingListModel>();


            int stop = 0;
            AllPalet.Add(data[0].Palet);
            
            for (int i = 1; i < data.Count; i++)
            {
                for (int j = 0; j < AllPalet.Count; j++)
                {
                    if (data[i].Palet == AllPalet[j])
                    {
                        for (int k = AllPalet.Count - 1; k > AllPalet.Count - 2; k--)
                        {
                            if (data[i].Palet == AllPalet[k])
                            {
                                AllPalet.Add(data[i].Palet);
                                data[i].Palet = "";
                                stop = 1;
                            }
                        }
                    }
                    if (stop == 1) break;
                }
                if (stop != 1) { AllPalet.Add(data[i].Palet); stop = 0; }
                stop = 0;
            }

            foreach (var row in data)
            {
                info.Add(new PackingListModel
                {
                    Palet = row.Palet,
                    Colet = row.Colet,
                    Material = row.Material,
                    DescriereMaterial = row.DescriereMaterial,
                    Cantitate = row.Cantitate
                });
            }
            return View(info);
        }




        //Login Part(Login, Validate(onclick Login btn), LogOut, GetPermission)
       
        public IActionResult Login()
        {
            return View();
        }


        

        
        public async Task<IActionResult> Validate(string username, string password)
        {

            try
            {
                var result = 0;//returnParameter.Value;
                var data = LoadUser();
                List<LoginModel> codes = new List<LoginModel>();
                foreach (var row in data)
                {
                       if(row.username==username && row.pass==password)result = 1;
                       if(result== 1) break;
                }
      
                if (result.Equals(1))
                {

                    var claims = new List<Claim>();
                    claims.Add(new Claim("username", username));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index");
                }
                TempData["Error"] = "Error1! User or Password is invalid";
                return RedirectToAction("Login");

            }
            catch
            {
                ViewBag.LoginError = "Error2! User or Password is invalid";
                return RedirectToAction("Login");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

    }


}

