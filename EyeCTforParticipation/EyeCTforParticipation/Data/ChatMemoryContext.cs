using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using EyeCTforParticipation.Data.MemoryContext;

namespace EyeCTforParticipation.Data
{
    class ChatMemoryContext : IChatContext
    {
        public void Clear(int chatId)
        {
            var results = from message in Tables.Message
                          where message.Chat.Id == chatId
                          select message;
            if(results.Count() == 1)
            {
                Tables.Message.Remove(results.ElementAt(0));
            }
        }

        public ChatModel Get(int chatId)
        {

            ChatModel chat = null;
            var results = from message in Tables.Message
                         where message.Chat.Id == chatId
                         join application in Tables.Application on message.Chat.Id equals application.Id
                         join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                         orderby message.Date descending
                         select new { Message = message, Application = application, HelpRequest = helpRequest };
            foreach(object result in results)
            {
                //http://stackoverflow.com/a/3358389
                Type type = result.GetType();
                MessageModel message = (MessageModel)type.GetProperty("Message").GetValue(result, null);
                ApplicationModel application = (ApplicationModel)type.GetProperty("Application").GetValue(result, null);
                HelpRequestModel helpRequest = (HelpRequestModel)type.GetProperty("HelpRequest").GetValue(result, null);

                if (chat == null)
                {
                    chat = (new ChatModel
                    {
                        Id = application.Id,
                        Title = helpRequest.Title,
                        Status = application.Status,
                        Messages = new List<MessageModel>()
                    });
                }
                chat.Messages.Add(new MessageModel
                {
                    Id = message.Id,
                    User = new UserModel
                    {
                        Id = message.User.Id
                    },
                    Chat = new ChatModel
                    {
                        Id = application.Id
                    },
                    Content = message.Content,
                    Date = message.Date
                });
            }
            return chat;
        }
        
        public List<ChatModel> List()
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          select new ChatModel
                          {
                             Id = application.Id,
                             Title = helpRequest.Title,
                             Status = application.Status
                          };
            return results.ToList();
        }

        public List<ChatModel> ListAsAidWorkerUser(int userId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          join helpSeekerAidWorker in Tables.HelpSeekerAidWorker on helpRequest.HelpSeeker.Id equals helpSeekerAidWorker.HelpSeekerUserId
                          where helpSeekerAidWorker.AidWorkerUserId == userId
                          select new ChatModel
                          {
                              Id = application.Id,
                              Title = helpRequest.Title,
                              Status = application.Status
                          };
            return results.ToList();
        }

        public List<ChatModel> ListAsHelpSeeker(int userId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where helpRequest.HelpSeeker.Id == userId
                          select new ChatModel
                          {
                              Id = application.Id,
                              Title = helpRequest.Title,
                              Status = application.Status
                          };
            return results.ToList();
        }

        public List<ChatModel> ListAsVolunteer(int userId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.Volunteer.Id == userId
                          select new ChatModel
                          {
                              Id = application.Id,
                              Title = helpRequest.Title,
                              Status = application.Status
                          };
            return results.ToList();
        }

        public void SendMessage(MessageModel message)
        {
            int id = Tables.Message.Count() > 0 ? Tables.Message.Max(x => x.Id) + 1 : 1;
            Tables.Message.Add(new MessageModel
            {
                Id = id,
                User = message.User,
                Chat = message.Chat,
                Content = message.Content,
                Date = DateTime.Now
            });
        }
    }
}
