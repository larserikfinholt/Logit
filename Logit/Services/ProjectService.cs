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

namespace Logit.Services
{
    
    public class ProjectService : RestServiceBase<Project>
    {
        private readonly List<Project> projects = new List<Project> { new Project { Title = "Proj1", ProjectId = "projects/1" }, new Project { Title = "Proj2", ProjectId = "projects/2" } };

        public override object OnGet(Project request)
        {
            if (string.IsNullOrEmpty(request.ProjectId))
                return projects;

            return projects.First();
        }

        public override object OnPost(Project project)
        {
            var newProject = project;
            return new HttpResult(newProject)
            {
                StatusCode = HttpStatusCode.Created,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + "Project/"+project.ProjectId}
                }
            };
        }

        public override object OnPut(Project project)
        {
            var updated = project;
            return new HttpResult()
            {
                StatusCode = HttpStatusCode.NoContent,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash()  + "Project/"+project.ProjectId}
                }
            };
        }

    }

}