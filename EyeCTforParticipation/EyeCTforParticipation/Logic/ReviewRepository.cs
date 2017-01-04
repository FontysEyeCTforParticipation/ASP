using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Data;
using EyeCTforParticipation.Models;

namespace EyeCTforParticipation.Logic
{
    class ReviewRepository
    {
        IReviewContext context;

        public ReviewRepository(IReviewContext context)
        {
            this.context = context;
        }

        List<ReviewModel> Get(UserModel user)
        {
            switch (user.Role)
            {
                case UserRole.HelpSeeker:
                    return context.GetFromHelpSeeker(user.Id);
                case UserRole.Volunteer:
                    return context.GetFromVolunteer(user.Id);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Create a new review if the id is 0.
        /// Else if a review exists with the same id, the existing review will be updated.
        /// </summary>
        /// <param name="review">
        /// The new or updated review.
        /// </param>
        public ReviewModel Save(ReviewModel review)
        {
            if (review.Id == 0)
            {
                review.Id = context.Create(review);
            } else
            {
                context.Update(review);
            }
            return review;            
        }

        public ReviewReplyModel Save(ReviewReplyModel reviewReply)
        {
            context.SaveReply(reviewReply);
            return reviewReply;
        }

        /// <summary>
        /// Delete a review.
        /// </summary>
        /// <param name="reviewId">'
        /// Id of the review.
        /// </param>
        /// <param name="user">
        /// The user of the review.
        /// The review will only be deleted if the user:
        ///     - Is an Admin.
        ///     - Created the review.
        /// </param>
        public void Delete(int reviewId, UserModel user)
        {
            switch (user.Role)
            {
                case UserRole.Admin:
                    context.Delete(reviewId);
                    break;
                case UserRole.HelpSeeker:
                    context.DeleteAsHelpSeeker(reviewId, user.Id);
                    break;
            }
        }

        /// <summary>
        /// Add or update a reply to a review.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        public void Reply(ReviewReplyModel reply)
        {
            context.SaveReply(reply);
        }

        /// <summary>
        /// Delete a reply from a review.
        /// </summary>
        /// <param name="reviewId">
        /// Id of the review.
        /// </param>
        /// <param name="user">
        /// The user that wants to remove the reply.
        /// The reply will only be deleted from a review if the user:
        ///     - Is an Admin.
        ///     - Created the reply to the review.
        /// </param>
        public void DeleteReply(int reviewId, UserModel user)
        {
            switch (user.Role)
            {
                case UserRole.Admin:
                    context.DeleteReply(reviewId);
                    break;
                case UserRole.Volunteer:
                    context.DeleteReplyVolunteer(reviewId);
                    break;
            }
        }
    }
}
