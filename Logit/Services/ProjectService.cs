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

        private IDocumentSession RavenSession { get; set; }
        
        public ProjectService(IDocumentSession documentSession)
        {
            RavenSession = documentSession;
            user = RavenSession.GetCurrentUser();
        }

        public override object OnGet(Project request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return RavenSession.Query<Project>().Where(x => x.Owner == user.Id);

            return RavenSession.Query<Project>().Where(x => x.Id == request.Id);
        }

        public override object OnPost(Project project)
        {
            project.Owner = user.Id;

            RavenSession.Store(project);
            RavenSession.SaveChanges();
            

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
            RavenSession.Store(project);
            RavenSession.SaveChanges();
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