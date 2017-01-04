using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;

namespace EyeCTforParticipation.Data
{
    interface IReviewContext
    {
        List<ReviewModel> GetFromHelpSeeker(int id);
        List<ReviewModel> GetFromVolunteer(int id);
        int Create(ReviewModel review);
        void Update(ReviewModel review);
        void SaveReply(ReviewReplyModel reviewreplymodel);
        void Delete(int id);
        void DeleteAsHelpSeeker(int id, int userId);
        void DeleteReply(int id);
        void DeleteReplyVolunteer(int id);
    }
}
