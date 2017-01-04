using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Data;
using EyeCTforParticipation.Models;

namespace EyeCTforParticipation.Logic
{
    class ChatRepository
    {
        IChatContext context;

        public ChatRepository(IChatContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get a list of chats.
        /// </summary>
        /// <param name="userId">
        /// The id of the user that requests the list of chats.
        /// </param>
        /// <returns>
        /// A list of chats for the specific user id.
        /// </returns>
        /// <remarks>
        /// List consists of ChatModel objects with Application.Id as Id, HelpRequest.Title as Title and Application.Status as Status.
        /// Different user roles get different lists.
        /// </remarks>
        public List<ChatModel> List(UserModel user)
        {
            switch (user.Role)
            {
                case UserRole.Admin:
                    return context.List();
                case UserRole.AidWorker:
                    return context.ListAsAidWorkerUser(user.Id);
                case UserRole.HelpSeeker:
                    return context.ListAsHelpSeeker(user.Id);
                case UserRole.Volunteer:
                    return context.ListAsVolunteer(user.Id);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get a chat with a list of messages.
        /// </summary>
        /// <param name="chatId">
        /// The id of the chat.
        /// </param>
        /// <param name="userId">
        /// The id of the user that requests the chat.
        /// </param>
        /// <returns>
        /// A chat with a list of messages.
        /// </returns>
        /// <remarks>
        /// The ChatModel object has Application.Id as Id, HelpRequest.Title as Title, Application.Status as Status.
        /// </remarks>
        public ChatModel Get(int chatId)
        {
            return context.Get(chatId);
        }

        /// <summary>
        /// Remove all messages from the chat.
        /// </summary>
        /// <param name="chatId">
        /// The id of the chat.
        /// </param>
        public void Clear(int chatId)
        {
            context.Clear(chatId);
        }

        /// <summary>
        /// Send a chat message.
        /// </summary>
        /// <param name="message">
        /// The message to be sent.
        /// </param>
        public void SendMessage(MessageModel message)
        {
            context.SendMessage(message);
        }
    }
}
