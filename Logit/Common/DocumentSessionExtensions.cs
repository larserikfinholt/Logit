using Logit.Models;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logit.Common
{
    public static class DocumentSessionExtensions
    {
       
        public static User GetCurrentUser(this IDocumentSession session)
        {
            if (HttpContext.Current.Request.IsAuthenticated == false)
                return null;

            var userName = HttpContext.Current.User.Identity.Name;
            var user = session.GetUserByUserName(userName);
            return user;
        }

        public static User GetUserByEmail(this IDocumentSession session, string email)
        {
            return session.Query<User>().FirstOrDefault(u => u.Email == email);
        }

        public static User GetUserByUserName(this IDocumentSession session, string userName)
        {
            return session.Query<User>().FirstOrDefault(u => u.UserName == userName);
        }
        

    }
}