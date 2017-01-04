using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class HelpSeekerAidWorkerModel
    {
        int helpSeekerUserId;
        int aidWorkerUserId;
        bool approved;

        public int HelpSeekerUserId
        {
            get
            {
                return helpSeekerUserId;
            }

            set
            {
                helpSeekerUserId = value;
            }
        }

        public int AidWorkerUserId
        {
            get
            {
                return aidWorkerUserId;
            }

            set
            {
                aidWorkerUserId = value;
            }
        }

        public bool Approved
        {
            get
            {
                return approved;
            }

            set
            {
                approved = value;
            }
        }
    }
}
