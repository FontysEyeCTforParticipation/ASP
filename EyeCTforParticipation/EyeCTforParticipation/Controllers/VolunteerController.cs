using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EyeCTforParticipation.Logic;
using EyeCTforParticipation.Models;
using EyeCTforParticipation.Data;
using System.Device.Location;

namespace EyeCTforParticipation.Controllers
{
    public class VolunteerController : Controller
    {
        HelpRequestRepository helpRequestRepository = new HelpRequestRepository(new HelpRequestSQLContext());

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search(string keywords = null, string postalCode = null, SearchDistance distance = SearchDistance.ALL, SearchOrder order = SearchOrder.DATE_DESC, int page = 1)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.Volunteer)
            {
                SearchResultModel result;

                postalCode = helpRequestRepository.FormatPostalCode(postalCode);
                if(postalCode == null)
                {
                    result = helpRequestRepository.Search(keywords, ((VolunteerModel)Session["volunteer"]).Location, distance, order, user.Id, (page - 1) * 10);
                } else
                {
                    result = helpRequestRepository.Search(keywords, postalCode, distance, order, user.Id, (page - 1) * 10);
                }
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["keywords"] = keywords;
                data["postalCode"] = postalCode;
                data["distance"] = distance;
                data["order"] = order;
                data["page"] = page;
                data["result"] = result;
                return View(data);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Helprequests()
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.Volunteer)
            {
                return View(helpRequestRepository.GetApplications(user.Id));
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Account()
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.Volunteer)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}