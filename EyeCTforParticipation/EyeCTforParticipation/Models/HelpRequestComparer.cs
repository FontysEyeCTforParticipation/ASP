using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class HelpRequestComparer : IComparer<HelpRequestModel>
    {
        HelpRequestCompare helpRequestCompare;

        public HelpRequestComparer(HelpRequestCompare helpRequestCompare)
        {
            this.helpRequestCompare = helpRequestCompare;
        }

        public int Compare(HelpRequestModel x, HelpRequestModel y)
        {
            switch (helpRequestCompare)
            {
                case HelpRequestCompare.DATE_ASC:
                    return x.Date.CompareTo(y.Date);
                case HelpRequestCompare.DATE_DESC:
                    return y.Date.CompareTo(x.Date);
                case HelpRequestCompare.DISTANCE_ASC:
                    return x.Distance.CompareTo(y.Distance);
                case HelpRequestCompare.DISTANCE_DESC:
                    return y.Distance.CompareTo(x.Distance);
                case HelpRequestCompare.TITLE_ASC:
                    return x.Title.CompareTo(y.Title);
                case HelpRequestCompare.TITLE_DESC:
                default:
                    return y.Title.CompareTo(x.Title);
            }
        }
    }
}
