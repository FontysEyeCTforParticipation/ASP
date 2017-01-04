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
    public class HelpRequestController : BaseController
    {
        HelpRequestRepository helpRequestRepository = new HelpRequestRepository(new HelpRequestSQLContext());

        [HttpGet]
        public ActionResult Get(int id)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                HelpRequestModel helpRequest = helpRequestRepository.Get(id);

                if (helpRequest != null)
                {
                    List<ApplicationModel> applications = helpRequestRepository.GetApplications(helpRequest.Id, user.Id);

                    return Json(new
                    {
                        Success = true,
                        HelpRequest = helpRequest,
                        Applications = applications
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);
        }
    }
}