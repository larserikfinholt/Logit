using Logit.Models;
using Raven.Client;
using ServiceStack.Text;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Logit.Common;

namespace Logit.Services
{

    public class ProjectService : RestServiceBase<Project>
    {
        private User user;

        private IDocumentSession Session { get; set; }
        
        public ProjectService(IDocumentSession documentSession)
        {
            Session = documentSession;
            user = Session.GetCurrentUser();
        }

        public override object OnGet(Project request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return Session.Query<Project>().Where(x => x.Owner == user.Id);

            return Session.Query<Project>().Where(x => x.Id == request.Id);
        }

        public override object OnPost(Project project)
        {
            project.Owner = user.Id;

            Session.Store(project);
            Session.SaveChanges();
            

            return new HttpResult(project)
            {
                StatusCode = HttpStatusCode.Created,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + project.Id}
                }
            };
        }

        public override object OnPut(Project project)
        {
            Session.Store(project);
            Session.SaveChanges();
            return new HttpResult(project)
            {
                StatusCode = HttpStatusCode.OK,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash()  + project.Id}
                }
            };
        }

    }

}