using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EyeCTforParticipation.Models
{
    public class NavModel
    {
        string title;
        string href;
        bool current;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string Href
        {
            get
            {
                return href;
            }

            set
            {
                href = value;
            }
        }

        public bool Current
        {
            get
            {
                return current;
            }

            set
            {
                current = value;
            }
        }
    }
}