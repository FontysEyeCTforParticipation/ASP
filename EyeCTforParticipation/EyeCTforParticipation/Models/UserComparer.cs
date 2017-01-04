using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class UserComparer : IComparer<UserModel>
    {
        UserCompare userCompare;

        public UserComparer(UserCompare userCompare)
        {
            this.userCompare = userCompare;
        }

        public int Compare(UserModel x, UserModel y)
        {
            switch (userCompare)
            {
                case UserCompare.NAME_ASC:
                    return x.Name.CompareTo(y.Name);
                case UserCompare.NAME_DESC:
                    return y.Name.CompareTo(x.Name);
                case UserCompare.ROLE_ASC:
                    return x.Role.CompareTo(y.Role);
                case UserCompare.ROLE_DESC:
                    return y.Role.CompareTo(x.Role);
                case UserCompare.AGE_ASC:
                    return x.Birthdate.CompareTo(y.Birthdate);
                case UserCompare.AGE_DESC:
                    return y.Birthdate.CompareTo(x.Birthdate);
                default:
                    return x.Name.CompareTo(y.Name);
            }
        }
    }
}
