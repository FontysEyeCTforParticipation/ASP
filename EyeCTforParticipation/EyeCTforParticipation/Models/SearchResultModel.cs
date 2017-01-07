using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EyeCTforParticipation.Models
{
    public class SearchResultModel
    {
        int count = 0;
        List<HelpRequestModel> results = new List<HelpRequestModel>();

        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }

        public List<HelpRequestModel> Results
        {
            get
            {
                return results;
            }

            set
            {
                results = value;
            }
        }
    }
}