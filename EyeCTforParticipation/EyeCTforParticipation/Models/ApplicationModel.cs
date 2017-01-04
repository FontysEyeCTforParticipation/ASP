using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class ApplicationModel
    {
        int id;
        HelpRequestModel helpRequest;
        VolunteerModel volunteer;
        ApplicationStatus status;
        DateTime date;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public HelpRequestModel HelpRequest
        {
            get
            {
                return helpRequest;
            }

            set
            {
                helpRequest = value;
            }
        }

        public VolunteerModel Volunteer
        {
            get
            {
                return volunteer;
            }

            set
            {
                volunteer = value;
            }
        }

        public ApplicationStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }
    }
}
