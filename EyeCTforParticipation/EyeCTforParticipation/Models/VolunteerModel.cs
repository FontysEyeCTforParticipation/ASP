using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace EyeCTforParticipation.Models
{
    public class VolunteerModel : UserModel
    {
        string about;
        string address;
        GeoCoordinate location;
        bool driversLicense;
        bool car;

        public string About
        {
            get
            {
                return about;
            }

            set
            {
                about = value;
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
