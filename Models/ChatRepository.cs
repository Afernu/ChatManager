using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ChatManager.Models
{
    public class ChatRepository : Repository<Chat>
    {
        public Chat Create(int diD, int riD, string message)
        {
            try
            {
                Chat chat = new Chat();
                {
                    chat.EnvoyeurId = diD;
                    chat.ReceveurId = riD;
                    chat.Message = message;
                    chat.MessageDate = DateTime.Now;
                }
                chat.Id = base.Add(chat);

                return chat;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add chat failed : Message - {ex.Message}");
            }
            return null;
        }
        public Chat Edit(string message,int id)
        {
            try
            {
                Chat chat = DB.Chat.FindChat(id);
                if (chat.EnvoyeurId == OnlineUsers.GetSessionUser().Id)
                {
                    chat.Message = message;
                    DB.Chat.Update(chat);
                }
                return chat;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Find chat failed : Message - {ex.Message}");
                return null;
            }
        }
        public Chat Delete_Chat(int id)
        {
            try
            {
                Chat chat = DB.Chat.FindChat(id);
                if(chat.EnvoyeurId == OnlineUsers.GetSessionUser().Id || OnlineUsers.GetSessionUser().IsAdmin)
                {
                    DB.Chat.Delete(id);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Find chat failed : Message - {ex.Message}");
                return null;
            }
        }
        public Chat FindChat(int id)
        {
            try
            {
                Chat chat = DB.Chat.ToList().Where(u => u.Id == id).FirstOrDefault();
                if (chat != null)
                    return DB.Chat.Get(id);

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Find chat failed : Message - {ex.Message}");
                return null;
            }
        }
    }
}