using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        #region Friendship Interactions
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {

            ViewBag.Search = SearchText;
            ViewBag.FilterRequest = FilterRequest;
            ViewBag.FilterNotFriend = FilterNotFriend;
            ViewBag.FilterPending = FilterPending;
            ViewBag.FilterFriend = FilterFriend;
            ViewBag.FilterRefused = FilterDenied;
            ViewBag.FilterBlocked = FilterBlocked;
            return View();
        }

        #endregion
        #region Friendships Handler
        public ActionResult SendFriendshipRequest(int id)
        {
            if (ModelState.IsValid)
            {
                DB.Friendships.Create(OnlineUsers.GetSessionUser().Id, id);
            }
            return null;
        }
        public ActionResult AcceptFriendshipRequest(int id)
        {
            if (ModelState.IsValid)
            {
                DB.Friendships.AcceptFriend(OnlineUsers.GetSessionUser().Id, id);
            }
            return null;
        }
        public ActionResult RemoveRequest(int id)
        {

            if (ModelState.IsValid)
            {
                DB.Friendships.Remove(OnlineUsers.GetSessionUser().Id, id);
            }
            return null;
        }
        public ActionResult DenyRequest(int id)
        {
            if (ModelState.IsValid)
            {
                DB.Friendships.Deny(OnlineUsers.GetSessionUser().Id, id);
            }
            return null;
        }
        #endregion
        #region Refresh and Lists
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetFriendShipsStatus(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                return PartialView(FilterUsers());
            }
            return null;
        }
        public ActionResult Search(string text)
        {
            SearchText = text.Trim().ToLower();
            //SearchText = "B".Trim().ToLower();
            return null;
        }
        private string SearchText
        {
            get
            {
                if (Session["Search"] == null)
                    Session["Search"] = string.Empty;
                return (string)Session["Search"];
            }
            set { Session["Search"] = value; }
        }
        private bool FilterNotFriend
        {
            get
            {
                if (Session["FilterNotFriend"] == null)
                    Session["FilterNotFriend"] = true;
                return (bool)Session["FilterNotFriend"];
            }
            set { Session["FilterNotFriend"] = value; }
        }

        private bool FilterDenied
        {
            get
            {
                if (Session["FilterRefused"] == null)
                    Session["FilterRefused"] = true;
                return (bool)Session["FilterRefused"];
            }
            set { Session["FilterRefused"] = value; }
        }

        private bool FilterBlocked
        {
            get
            {
                if (Session["FilterBlocked"] == null)
                    Session["FilterBlocked"] = true;
                return (bool)Session["FilterBlocked"];
            }
            set { Session["FilterBlocked"] = value; }
        }

        private bool FilterPending
        {
            get
            {
                if (Session["FilterPending"] == null)
                    Session["FilterPending"] = true;
                return (bool)Session["FilterPending"];
            }
            set { Session["FilterPending"] = value; }
        }
        private bool FilterFriend
        {
            get
            {
                if (Session["FilterFriend"] == null)
                    Session["FilterFriend"] = true;
                return (bool)Session["FilterFriend"];
            }
            set { Session["FilterFriend"] = value; }
        }
        private bool FilterRequest
        {
            get
            {
                if (Session["FilterRequest"] == null)
                    Session["FilterRequest"] = true;
                return (bool)Session["FilterRequest"];
            }
            set { Session["FilterRequest"] = value; }
        }

        public ActionResult SetFilterNotFriend(bool check)
        {
            FilterNotFriend = check;
            return null;
        }
        public ActionResult SetFilterFriend(bool check)
        {
            FilterFriend = check;
            return null;
        }
        public ActionResult SetFilterBlocked(bool check)
        {
            FilterBlocked = check;
            return null;
        }
        public ActionResult SetFilterRequest(bool check)
        {
            FilterRequest = check;
            return null;
        }
        public ActionResult SetFilterRefused(bool check)
        {
            FilterDenied = check;
            return null;
        }
        public ActionResult SetFilterPending(bool check)
        {
            FilterPending = check;
            return null;
        }

        private List<User> FilterUsers()
        {
            var loggedUserId = OnlineUsers.GetSessionUser().Id;
            List<User> filteredUsers = new List<User>();
            List<User> users = SearchText != "" ?
                DB.Users.SortedUsers().Where(u => (u.LastName + u.FirstName).ToLower().Contains(SearchText) && u.Verified).ToList() :
                DB.Users.SortedUsers().ToList().Where(u => u.Verified).ToList();

            foreach (var user in users)
            {
                bool keep = true;
                switch (DB.Friendships.FriendshipStatus(loggedUserId, user.Id))
                {
                    case 0:
                        keep = FilterPending;
                        break;
                    case 1: //accepted/friends
                        keep = FilterFriend;
                        break;
                    case 2: //Denied
                        keep = FilterDenied;
                        break;
                    case 3: //denied
                        keep = FilterNotFriend;
                        break;
                    case 4: //pending//
                        keep = FilterRequest;
                        break;
                    case 5:
                        keep = FilterFriend;
                        break;
                    case 6:
                        keep = FilterBlocked;
                        break;

                }
                if(keep)
                    filteredUsers.Add(user);
            }
            return filteredUsers;
        }

        #endregion
    }
}