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

    public class NoteService : RestServiceBase<Note>
    {
        private User user;

        private IDocumentSession RavenSession { get; set; }

        public NoteService(IDocumentSession documentSession)
        {
            RavenSession = documentSession;
            user = RavenSession.GetCurrentUser();
        }




        public override object OnGet(Note request)
        {
            if (string.IsNullOrEmpty(request.ProjectId))
                return new List<Note>();

            return RavenSession.Query<Note>().Where(x => x.ProjectId == request.ProjectId);
        }

        public override object OnPost(Note n)
        {
            var newNote = n;
            newNote.Text = n.Text ;
            newNote.Created = n.Created == new DateTime() ? DateTime.UtcNow : n.Created;
            newNote.LastUpated = DateTime.UtcNow;

            RavenSession.Store(newNote);
            RavenSession.SaveChanges();

            return new HttpResult(newNote)
            {
                StatusCode = HttpStatusCode.Created,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + newNote.Id}
                }
            };
        }

        public override object OnPut(Note note)
        {
            var updated = note;
            updated.Text = note.Text;
            RavenSession.Store(updated);
            RavenSession.SaveChanges();
            return new HttpResult(updated)
            {
                StatusCode = HttpStatusCode.OK,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + note.Id}
                }
            };
        }

    }


}