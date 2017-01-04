using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace EyeCTforParticipation.Models
{
    public class RegisterModel
    {
        UserRole role;
        string name;
        string email;
        string password;
        DateTime birthdate;
        Bitmap avatar;
        string vog;
        string address;
        GeoCoordinate location;
        bool driversLicense;
        bool car;

        public UserRole Role
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

        public string VOG
        {
            get
            {
                return vog;
            }

            set
            {
                vog = value;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        public GeoCoordinate Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        public bool DriversLicense
        {
            get
            {
                return driversLicense;
            }

            set
            {
                driversLicense = value;
            }
        }

        public bool Car
        {
            get
            {
                return car;
            }

            set
            {
                car = value;
            }
        }
    }
}
