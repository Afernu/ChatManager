using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendships
    {
        #region Data Members
        public int Id { get; set; }
        public int DemandantId { get; set; }
        public int ReceveurId { get; set; }
        public static bool Accepted { get; set; } = false;
        public static bool Denied { get; set; } = false;
        public static bool IsPending { get; set; } = false;
        public bool IsFriends { get; set; }
        public bool IsUnknown { get; set; } = true;
        public bool IsRequesting { get; set; }
        public bool[] States { get; set; }
        #endregion

        public Friendships()
        {
            States = new bool[] { IsPending, Accepted, Denied };
        }

        [JsonIgnore] public User Demandant { get { return DB.Users.Get(DemandantId); } }
        [JsonIgnore] public User Receveur { get { return DB.Users.Get(ReceveurId); } }
        public static List<Friendships> ListFriend
        {
            get
            {
                return DB.Friendships.ToList();
            }
        }
        public static int Relationship(int dId, int rId)
        {
            return DB.Friendships.FriendshipStatus(dId, rId);
        }

    }
}