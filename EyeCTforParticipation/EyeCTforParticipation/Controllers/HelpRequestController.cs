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
    public class HelpRequestController : Controller
    {
        HelpRequestRepository helpRequestRepository = new HelpRequestRepository(new HelpRequestSQLContext());

        [HttpGet]
        public ActionResult Get(int id)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                HelpRequestModel helpRequest;
                switch (user.Role)
                {
                    case UserRole.HelpSeeker:
                        helpRequest = helpRequestRepository.Get(id);

                        if (helpRequest != null)
                        {
                            List<ApplicationModel> applications = helpRequestRepository.GetApplications(helpRequest.Id, user.Id);

                            return Json(new
                            {
                                helpRequest = helpRequest,
                                applications = applications
                            }, JsonRequestBehavior.AllowGet);
                        }
                        break;
                    case UserRole.Volunteer:
                        helpRequest = helpRequestRepository.Get(id, user.Id);

                        if (helpRequest != null)
                        {
                            return Json(helpRequest, JsonRequestBehavior.AllowGet);
                        }
                        break;
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Open(int id)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                helpRequestRepository.Open(id, user.Id);
            }
        }

        [HttpPost]
        public void Close(int id)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                helpRequestRepository.Close(id, user.Id);
            }
        }

        [HttpPost]
        public void Save(string title, HelpRequestUrgency urgency, string address, string content, int id = 0)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                helpRequestRepository.Save(new HelpRequestModel
                {
                    Id = id,
                    Title = title,
                    Urgency = urgency,
                    Address = address,
                    Content = content,
                    HelpSeeker = new UserModel
                    {
                        Id = user.Id
                    }
                });
            }
        }

        [HttpPost]
        public void Interview(int applicationId)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                helpRequestRepository.InterviewApplication(applicationId, user.Id);
            }
        }

        [HttpPost]
        public void Approve(int applicationId)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.HelpSeeker)
            {
                helpRequestRepository.ApproveApplication(applicationId, user.Id);
            }
        }

        [HttpPost]
        public void Apply(int id)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null && user.Role == UserRole.Volunteer)
            {
                helpRequestRepository.Apply(id, user.Id);
            }
        }

        [HttpPost]
        public void Cancel(int applicationId)
        {
            UserModel user = (UserModel)Session["user"];

            if (user != null)
            {
                switch (user.Role)
                {
                    case UserRole.Volunteer:
                        helpRequestRepository.CancelApplication(applicationId, user.Id);
                        break;
                    case UserRole.HelpSeeker:
                        helpRequestRepository.CancelApplicationAsHelpSeeker(applicationId, user.Id);
                        break;
                }
            }
        }
    }
}