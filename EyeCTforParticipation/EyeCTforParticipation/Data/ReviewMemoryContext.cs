using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;

namespace EyeCTforParticipation.Data
{
    class ReviewMemoryContext : IReviewContext
    {
        public int Create(ReviewModel review)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsHelpSeeker(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteReply(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteReplyVolunteer(int id)
        {
            throw new NotImplementedException();
        }

        public List<ReviewModel> GetFromHelpSeeker(int id)
        {
            throw new NotImplementedException();
        }

        public List<ReviewModel> GetFromVolunteer(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveReply(ReviewReplyModel reviewreplymodel)
        {
            throw new NotImplementedException();
        }

        public void Update(ReviewModel review)
        {
            throw new NotImplementedException();
        }
    }
}
