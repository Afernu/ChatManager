using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Chat
    {
        public Chat()
        {
            MessageDate = DateTime.Now;
        }
        #region Data Members
        public int Id { get; set; }
        public int EnvoyeurId { get; set; }
        public int ReceveurId { get; set; }
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }
        #endregion
        public Chat Clone()
        {
            return JsonConvert.DeserializeObject<Chat>(JsonConvert.SerializeObject(this));
        }
        
    }


}