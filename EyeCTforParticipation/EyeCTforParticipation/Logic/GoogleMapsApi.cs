using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using System.Net;
using Newtonsoft.Json;
using System.Device.Location;

namespace EyeCTforParticipation.Logic
{
    class GoogleMapsApi
    {
        static public Response Get(string address, string countryCode, string languageCode)
        {
            Response response;
            using (WebClient wc = new WebClient())
            {
                try
                {
                    response = JsonConvert.DeserializeObject<Response>(wc.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?language=" + languageCode + "&components=country:" + countryCode + "&address=" + address));
                }
                catch (Exception)
                {
                    response = null;
                }
            }
            if (response != null && response.Succes)
            {
                return response;
            }
            return null;
        }
        public class Response
        {

            public GeoCoordinate Location;
            public string Address;
            public bool Succes;

            public class Result
            {
                public class Geometry
                {
                    public class Location
                    {
                        public double lat;
                        public double lng;
                    }
                    public Location location;
                }
                public Geometry geometry;

                public string formatted_address;
            }

            public List<Result> results
            {
                set
                {
                    if(value != null && value.Count() > 0)
                    {
                        Location = new GeoCoordinate(value[0].geometry.location.lat, value[0].geometry.location.lng);
                        Address = value[0].formatted_address;
                    }
                }
            }

            public string Status
            {
                set
                {
                    Succes = value == "OK" ? true : false;
                }
            }
        }
    }
}
