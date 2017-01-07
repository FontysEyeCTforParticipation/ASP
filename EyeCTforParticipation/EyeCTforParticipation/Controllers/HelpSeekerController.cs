using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EyeCTforParticipation.Logic;
using EyeCTforParticipation.Data;
using EyeCTforParticipation.Models;
using Newtonsoft.Json;

namespace EyeCTforParticipation.Controllers
{
    public class HelpSeekerController : Controller
    {
        HelpRequestRepository helpRequestRepository = new HelpRequestRepository(new HelpRequestSQLContext());

        public ActionResult Index()
        {
            return RedirectToAction("HelpRequests");
        }

        public ActionResult HelpRequests()
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                List<HelpRequestModel> helpRequests = helpRequestRepository.GetFromHelpSeeker(user.Id);
                return View(helpRequests);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Account()
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}