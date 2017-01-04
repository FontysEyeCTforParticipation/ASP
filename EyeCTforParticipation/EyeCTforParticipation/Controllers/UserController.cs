using EyeCTforParticipation.Data;
using EyeCTforParticipation.Logic;
using EyeCTforParticipation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EyeCTforParticipation.Controllers
{
    public class UserController : Controller
    {
        UserRepository userRepository = new UserRepository(new UserSQLContext());

        [HttpPost]
        public ActionResult Login(string email, string password)
        {

            UserModel user = userRepository.Login(email, password);
            if (user != null)
            {
                Session["user"] = user;

                return Json(new
                {
                    Success = true,
                    Role = user.Role
                });
            }

            return Json(new
            {
                Success = false
            });
        }
        
        public ActionResult Logout()
        {
            Session["user"] = null;

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
                user.Zoom = zoom;
                Session["user"] = user;
            }
        }
    }
}