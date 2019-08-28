using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wedding_Planner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register(User newuser)
        {
            if(ModelState.IsValid == true)
            {
                dbContext.Users.Add(newuser);
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                dbContext.SaveChanges();
                User thisuser = dbContext.Users.FirstOrDefault(u => u.Email == newuser.Email);
                int UseID = thisuser.UserId;
                HttpContext.Session.SetInt32("loggedin", UseID);
                return RedirectToAction("UserPage", new{UseID = UseID});
            }
            else if(ModelState.IsValid == false)
            {
                return View("Index");
            }
            else
            {
                return View("Index");
            }
        }

        [Route("login/process")]
        [HttpPost]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid == true)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                    if(result == 0)
                    {
                        ModelState.AddModelError("Password", "Invalid Password");
                        return View("Index");
                    }
                    else
                    {
                        User thisuser = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                        int UseID = thisuser.UserId;
                        HttpContext.Session.SetInt32("loggedin", UseID);
                        return RedirectToAction("UserPage", new{UseID = UseID});
                    }
                }
            }
            else
            {
                return View("login");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [Route("userpage/{UseID:int}")]
        [HttpGet]
        public IActionResult UserPage(int UseID)
        {
            List<Wedding> AllWeddings = dbContext.Weddings.Include(w => w.TheseUsers).ToList();
            User thisuser = dbContext.Users.Include(uw => uw.TheseWeddings).ThenInclude(u => u.Wedding).FirstOrDefault(id => id.UserId == UseID);
            ViewBag.All = AllWeddings;
            ViewBag.loggedin = HttpContext.Session.GetInt32("loggedin");
            return View("UserPage", AllWeddings);
        }

        [Route("newwedding/{UseID:int}")]
        [HttpGet]
        public IActionResult NewWedding(int UseID)
        {
            ViewBag.loggedin = HttpContext.Session.GetInt32("loggedin");
            return View("NewWedding");
        }

        [Route("createnew/{UseID:int}")]
        [HttpPost]
        public IActionResult CreateNew(Wedding thiswedding, int UseID)
        {
            dbContext.Weddings.Add(thiswedding);
            dbContext.SaveChanges();
            var join = dbContext.Users.Include(uw => uw.TheseWeddings).ThenInclude(u => u.Wedding).FirstOrDefault(id => id.UserId == UseID);
            ViewBag.loggedin = HttpContext.Session.GetInt32("loggedin");
            return RedirectToAction("UserPage");
        }

        [Route("viewwedding/{WeddID:int}")]
        [HttpGet]
        public IActionResult ViewWedding(int WeddID)
        {
            Wedding thiswedding = dbContext.Weddings.Include(uw => uw.TheseUsers).ThenInclude(u => u.User).FirstOrDefault(w => w.WeddingId == WeddID);
            ViewBag.All = thiswedding;
            ViewBag.loggedin = HttpContext.Session.GetInt32("loggedin");
            return View("ViewWedding", thiswedding);
        }

        [Route("delete/{WeddID:int}")]
        [HttpGet]
        public IActionResult Delete(int WeddID)
        {
            Wedding thiswedding = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == WeddID);
            dbContext.Weddings.Remove(thiswedding);
            dbContext.SaveChanges();
            int UseID = HttpContext.Session.GetInt32("loggedin") ?? 0;
            return RedirectToAction("UserPage", new{UseID = UseID});
        }

        [Route("rsvp/{WeddID:int}")]
        [HttpGet]
        public IActionResult RSVP(int WeddID)
        {
            int UseID = HttpContext.Session.GetInt32("loggedin") ?? 0;
            Wedding weddingrsvp = dbContext.Weddings.Include(uw => uw.TheseUsers).ThenInclude(u => u.User).FirstOrDefault(w => w.WeddingId == WeddID);
            UserWedding newjoin = new UserWedding(UseID, WeddID);
            dbContext.UserWeddings.Add(newjoin);
            dbContext.SaveChanges();
            return RedirectToAction("UserPage", new{UseID = UseID});
        }

        [Route("unrsvp/{WeddID:int}")]
        [HttpGet]
        public IActionResult UNRSVP(int WeddID)
        {
            int UseID = HttpContext.Session.GetInt32("loggedin") ?? 0;
            Wedding weddingrsvp = dbContext.Weddings.Include(uw => uw.TheseUsers).ThenInclude(u => u.User).FirstOrDefault(w => w.WeddingId == WeddID);
            User guest = dbContext.Users.Include(y => y.TheseWeddings).ThenInclude(we => we.Wedding).FirstOrDefault(use => use.UserId == UseID);
            UserWedding unrsvp = dbContext.UserWeddings.Where(wedding => wedding.WeddingId == WeddID).FirstOrDefault(user => user.UserId == UseID);
            dbContext.UserWeddings.Remove(unrsvp);
            dbContext.SaveChanges();
            return RedirectToAction("UserPage", new{UseID = UseID});
        }

    }
}
