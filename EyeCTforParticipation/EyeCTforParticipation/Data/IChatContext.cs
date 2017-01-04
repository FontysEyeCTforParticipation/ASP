using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;

namespace EyeCTforParticipation.Data
{
    interface IChatContext
    {
        List<ChatModel> List();
        List<ChatModel> ListAsVolunteer(int userId);
        List<ChatModel> ListAsHelpSeeker(int userId);
        List<ChatModel> ListAsAidWorkerUser(int userId);
        ChatModel Get(int chatId);
        void Clear(int chatId);
        void SendMessage(MessageModel message);
    }
}
