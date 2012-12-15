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
        private readonly List<Note> notes = new List<Note> { new Note { Text = "Text1", Id = "notes/1", ProjectId = "projects/1" }, new Note { Text = "Text2", Id = "notes/2", ProjectId = "projects/1" } };

        private User user;

        private IDocumentSession Session { get; set; }

        public NoteService(IDocumentSession documentSession)
        {
            Session = documentSession;
            user = Session.GetCurrentUser();
        }




        public override object OnGet(Note request)
        {
            if (string.IsNullOrEmpty(request.ProjectId))
                return new List<Note>();

            return Session.Query<Note>().Where(x => x.ProjectId == request.ProjectId);
        }

        public override object OnPost(Note n)
        {
            var newNote = n;
            newNote.Text = n.Text ;

            Session.Store(newNote);
            Session.SaveChanges();

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
            Session.Store(updated);
            Session.SaveChanges();
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