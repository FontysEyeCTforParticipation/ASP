using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCTforParticipation.Models
{
    public class MessageModel
    {
        int id;
        UserModel user;
        ChatModel chat;
        string content;
        DateTime date;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public UserModel User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        internal ChatModel Chat
        {
            get
            {
                return chat;
            }

            set
            {
                chat = value;
            }
        }

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }
    }
}
