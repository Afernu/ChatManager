using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace ChatManager.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            ViewBag.Message = MessageText;
            ViewBag.CurrentTarget = CurrentTarget;
            return View();
        }
        private List<User> FilterUsers()
        {
            var loggedUserId = OnlineUsers.GetSessionUser().Id;

            List<User> users = DB.Users.SortedUsers().Where(u => u.Verified).ToList();

            List<int> acceptedStatuses = new List<int> { 1, 5 };

            List<User> friendUsers = users.Where(user => acceptedStatuses.Contains(DB.Friendships.FriendshipStatus(loggedUserId, user.Id)))
                                          .ToList();

            return friendUsers;
        }

        #region Refresh
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetFriendsList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Friendships.HasChanged)
            {
                return PartialView(FilterUsers());
            }
            return null;
        }
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetMessages(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged() || DB.Chat.HasChanged)
            {
                int? selectedTarget = CurrentTarget; // Retrieve the selected target ID
                return PartialView(FilterMessage(selectedTarget));
            }

            return null;
        }


        private List<Chat> FilterMessage(int? select)
        {
            List<Chat> chat = DB.Chat.ToList();
            var loggedUserId = OnlineUsers.GetSessionUser().Id;

            // Filter the messages based on the selected target
            List<Chat> listChat;
            if (select != 0)
            {
                listChat = chat.Where(c => (c.EnvoyeurId == loggedUserId && c.ReceveurId == select) || (c.EnvoyeurId == select && c.ReceveurId == loggedUserId)).ToList();
            }
            else
            {
                listChat = chat.Where(c => c.EnvoyeurId == loggedUserId).ToList();
            }

            // Order the messages by time in descending order (latest messages first)
            listChat = listChat.OrderBy(c => c.MessageDate).ToList();

            return listChat;
        }



        #endregion

        #region Message
        public JsonResult IsTargetTyping()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StopTyping()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsTyping()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Message(string message)
        {
            MessageText = message.Trim().ToLower();
            //SearchText = "B".Trim().ToLower();
            return null;
        }
        public ActionResult Send(string message)
        {
            if(ModelState.IsValid)
                DB.Chat.Create(OnlineUsers.GetSessionUser().Id, CurrentTarget, message);
            return null;
        }
        public ActionResult Update(int id, string message)
        {
            if (ModelState.IsValid)
                DB.Chat.Edit(message, id);
            return null;
        }
        public Action Delete(int id)
        {
            if (ModelState.IsValid)
                DB.Chat.Delete_Chat(id);
            return null;
        }
        public ActionResult SetCurrentTarget(int id)
        {
            if (ModelState.IsValid)
            {
                CurrentTarget = id;
            }
            return null;
        }

        private int CurrentTarget
        {
            get
            {
                if (Session["CurrentId"] == null)
                    Session["CurrentId"] = 0;
                return (int)Session["CurrentId"];
            }
            set { Session["CurrentId"] = value; }

        }

        private string MessageText
        {
            get
            {
                if (Session["Message"] == null)
                    Session["Message"] = string.Empty;
                return (string)Session["Message"];
            }
            set { Session["Message"] = value; }
        }

        #endregion


    }
}