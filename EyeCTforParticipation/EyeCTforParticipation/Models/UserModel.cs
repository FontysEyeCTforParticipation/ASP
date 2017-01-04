using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class UserModel
    {
        int id;
        UserRole role;
        string name;
        string email;
        string rfid;
        string password;
        DateTime birthdate;
        bool approved;
        Bitmap avatar;
        int zoom;

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

        public virtual UserRole Role
        {
            get
            {
                return role;
            }

            set
            {
                role = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Rfid
        {
            get
            {
                return rfid;
            }

            set
            {
                rfid = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public DateTime Birthdate
        {
            get
            {
                return birthdate;
            }

            set
            {
                birthdate = value;
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

        public Bitmap Avatar
        {
            get
            {
                return avatar;
            }

            set
            {
                avatar = value;
            }
        }

        public int Zoom
        {
            get
            {
                return zoom;
            }

            set
            {
                zoom = value;
            }
        }
    }
}
