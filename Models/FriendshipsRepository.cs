using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipsRepository : Repository<Friendships>
    {
        public Friendships Create(int idDemandant, int idAccepteur)
        {
            Friendships friend = DB.Friendships.ToList()
                    .Where(u => (u.DemandantId == idAccepteur && u.ReceveurId == idDemandant) || (u.DemandantId == idDemandant && u.ReceveurId == idAccepteur))
                    .FirstOrDefault();
            try
            {
                if (friend != null)
                {
                    friend.IsRequesting = true;
                    friend.IsUnknown = false;
                    friend.DemandantId = idDemandant;
                    friend.ReceveurId = idAccepteur;
                    friend.States = new bool[] { true, false, false };
                    DB.Friendships.Update(friend);
                }
                else
                {
                    friend = new Friendships
                    {
                        IsRequesting = true,
                        IsUnknown = false,
                        DemandantId = idDemandant,
                        ReceveurId = idAccepteur,
                        States = new bool[] { true, false, false }
                    };
                    friend.Id = base.Add(friend);
                }
                return friend;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add friendships failed : Message - {ex.Message}");
            }
            return null;
        }
        public Friendships FindFriendship(int id)
        {
            try
            {
                Friendships friend = DB.Friendships.Get(id);
                if (friend != null)
                    return friend;
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Find user failed : Message - {ex.Message}");
                return null;
            }
        }
        //             States = new bool[] { IsPending, IsFriends, Accepted, Denied, IsUnknown };

        public void AcceptFriend(int idAccepteur, int idDemandant)
        {
            try
            {
                Friendships friend = DB.Friendships.ToList().Where(u => u.DemandantId == idDemandant && u.ReceveurId == idAccepteur).FirstOrDefault();
                if (friend != null)
                {
                    BeginTransaction();
                    for (int i = 0; i < friend.States.Length; i++)
                        friend.States[i] = false;

                    friend.IsRequesting = false;
                    friend.IsFriends = true;
                    friend.States[1] = true;
                    DB.Friendships.Update(friend);
                    EndTransaction();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Accept failed : Message - {ex.Message}");
            }
        }
        public void Remove(int idAccepteur, int idDemandant)
        {
            try
            {
                Friendships friend = DB.Friendships.ToList()
                    .Where(u => (u.DemandantId == idAccepteur && u.ReceveurId == idDemandant) || (u.DemandantId == idDemandant && u.ReceveurId == idAccepteur))
                    .FirstOrDefault();
                if (friend != null)
                {
                    BeginTransaction();
                    //for (int i = 0; i < friend.States.Length; i++)
                    //    friend.States[i] = false;

                    //friend.States[4] = true;
                    DB.Friendships.Delete(friend.Id);
                    EndTransaction();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Accept failed : Message - {ex.Message}");
            }
        }
        public void Deny(int idAccepteur, int idDemandant)
        {
            try
            {
                Friendships friend = DB.Friendships.ToList()
                    .Where(u => (u.DemandantId == idAccepteur && u.ReceveurId == idDemandant) || (u.DemandantId == idDemandant && u.ReceveurId == idAccepteur))
                    .FirstOrDefault();
                if (friend != null)
                {
                    BeginTransaction();
                    for (int i = 0; i < friend.States.Length; i++)
                        friend.States[i] = false;

                    friend.IsRequesting = false;
                    friend.States[2] = true;
                    DB.Friendships.Update(friend);
                    EndTransaction();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Accept failed : Message - {ex.Message}");
            }
        }
        public int FriendshipStatus(int demandantId, int receveurId)
        {
            int trueIndex;
            var friend = DB.Friendships.ToList()
                    .Where(u => (u.DemandantId == receveurId && u.ReceveurId == demandantId) || (u.DemandantId == demandantId && u.ReceveurId == receveurId))
                    .FirstOrDefault();
            if (friend != null)
                trueIndex = Array.IndexOf(friend.States, true);
            else
                if (DB.Users.FindUser(receveurId).Blocked)
                return 6;
            else
                return 3;

            if (trueIndex == 0 && friend.DemandantId == OnlineUsers.GetSessionUser().Id)
                return 0;
            if (friend.IsRequesting)
                return 4;
            if (friend.IsFriends)
                return 5;
            return trueIndex;
        }
        public Friendships Relation(int demandantId, int receveurId)
        {
            Friendships friend = DB.Friendships.ToList()
            .FirstOrDefault(u => (u.DemandantId == demandantId && u.ReceveurId == receveurId) || (u.DemandantId == receveurId && u.ReceveurId == demandantId));
            if (friend != null)
            {
                return friend;
            }
            else
            {
                return null;
            }

        }
    }
}