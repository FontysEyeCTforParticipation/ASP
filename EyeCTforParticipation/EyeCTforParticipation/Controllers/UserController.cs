using EyeCTforParticipation.Data;
using EyeCTforParticipation.Logic;
using EyeCTforParticipation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EyeCTforParticipation.Controllers
{
    public class UserController : Controller
    {
        UserRepository userRepository = new UserRepository(new UserSQLContext());

        public ActionResult Login(string email = null, string password = null)
        {
            if (email != null && password != null)
            {
                UserModel user = userRepository.Login(email, password);
                if(user != null)
                {
                    Session["user"] = user;
                    if(user.Role == UserRole.Volunteer)
                    {
                        Session["volunteer"] = userRepository.GetVolunteer(user.Id);
                    }
                    return Json(true);
                }
            }

            return Json(false);
        }

        public ActionResult Account()
        {
            UserModel user = (UserModel)Session["user"];
            if (user != null)
            {
                switch (user.Role)
                {
                    case UserRole.HelpSeeker:
                        return RedirectToAction("Index", "HelpSeeker");
                    case UserRole.Volunteer:
                        return RedirectToAction("Index", "Volunteer");
                }
            }

            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult Logout()
        {
            Session["user"] = null;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public void Profile(string name, string birthdate, string address = "", bool driversLicense = false, bool car = false, string about = "")
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                DateTime date = Convert.ToDateTime(birthdate);
                userRepository.Profile(name, date, user.Id);
                Session["user"] = userRepository.Get(((UserModel)Session["user"]).Id);
                if(user.Role == UserRole.Volunteer)
                {
                    userRepository.VolunteerProfile(address, driversLicense, car, about, user.Id);
                    Session["volunteer"] = userRepository.GetVolunteer(((UserModel)Session["user"]).Id);
                }
            }
        }

        [HttpPost]
        public ActionResult Email(string email, string password)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                if (userRepository.CheckPassword(password, user.Password))
                {
                    if(userRepository.ValidateEmail(email))
                    {
                        userRepository.Email(email, user.Id, Request.UrlReferrer.ToString(), user.Email, user.Name);
                        return Json(true);
                    }
                    return Json("InvalidEmail");

                }
                return Json("InvalidPassword");
            }

            return Json(null);
        }

        [HttpPost]
        public ActionResult Password(string password, string newPassword)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                if (userRepository.CheckPassword(password, user.Password))
                {
                    userRepository.Password(newPassword, user.Id, Request.UrlReferrer.ToString(), user.Email, user.Name);

                    return Json(true);
                }
                return Json(false);
            }

            return Json(null);
        }
        
        public ActionResult Confirm(string id)
        {
            try
            {
                string base64 = userRepository.Token(id);
                Dictionary<string, object> data = userRepository.Base64ToDictionary(base64);
                try {
                    string action = (string)data["action"];
                    string url = (string)data["url"];

                    switch (action)
                    {
                        case "password":
                            int userId = Convert.ToInt32(data["userId"]);
                            string hash = (string)data["hash"];
                            userRepository.Password(hash, userId);
                            TempData["popupTitle"] = "Wachtwoord wijzigen";
                            TempData["popupMessage"] = "Wachtwoord is gewijzigd.";
                            break;
                    }

                    UserModel user = (UserModel)Session["user"];

                    if (user != null)
                    {
                        Session["user"] = userRepository.Get(user.Id);
                    }

                    return Redirect(url);
                }
                catch (KeyNotFoundException)
                {
                    TempData["popupTitle"] = "Error";
                    TempData["popupMessage"] = "Er is iets mis gegaan, het probleem is gelogd en wordt zo spoedig mogelijk verholpen.";
                    
                    //Log corrupt data to console
                    Trace.TraceError("Invalid token data, following output is base64 string of data:" + base64);
                }
            }
            catch (InvalidTokenException)
            {
                TempData["popupTitle"] = "Error";
                TempData["popupMessage"] = "Code is verlopen of ongeldig.";
            }
            catch (FormatException)
            {
                TempData["popupTitle"] = "Error";
                TempData["popupMessage"] = "Code is ongeldig. Controleer of u de link correct heeft ingevoerd.";
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public void Zoom(int zoom)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                zoom = zoom >= 100 ? zoom <= 150 ? zoom : 150 : 100;
                userRepository.Zoom(zoom, user.Id);
                Session["user"] = userRepository.Get(((UserModel)Session["user"]).Id);
            }
        }
    }
}