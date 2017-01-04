using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class ReviewModel
    {
        int id;
        UserModel helpSeeker;
        VolunteerModel volunteer;
        string content;
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

        public UserModel HelpSeeker
        {
            get
            {
                return helpSeeker;
            }

            set
            {
                helpSeeker = value;
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

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
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
