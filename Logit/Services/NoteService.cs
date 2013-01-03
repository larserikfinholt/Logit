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
        protected NoteService(IDocumentSession documentSession, User _user)
        {
            RavenSession = documentSession;
            user = _user;
        }

        public static NoteService CreateNoteService(IDocumentSession documentSession, User _user)
        {
            return new NoteService(documentSession, _user);
        }



        public override object OnGet(Note request)
        {
            if (string.IsNullOrEmpty(request.ProjectId))
                return new List<Note>();

            return RavenSession.Query<Note>().Where(x => x.ProjectId == request.ProjectId);
        }

        public override object OnPost(Note n)
        {
            var project = RavenSession.Load<Project>(n.ProjectId);
            var note = n.Id==null?n:RavenSession.Load<Note>(n.Id);
            var newNote = note;
            newNote.Text = n.Text ;
            newNote.Created = n.Created == new DateTime() ? DateTime.UtcNow : n.Created;
            newNote.LastUpated = DateTime.UtcNow;

            RavenSession.Store(newNote);

            // Add a reminder
            if (project.MaxNoteIntervalInDaysBeforeReminder > 0)
            {
                var reminder = RavenSession.Query<Reminder>().Where(r => r.ProjectId == n.ProjectId && r.RemindersSent == 0).FirstOrDefault();
                if (reminder == null)
                {
                    reminder = new Reminder { ProjectId = project.Id, ProjectTitle = project.Title, UserId = project.Owner, FullName=user.Email };
                }
                reminder.LastNoteText = n.Text;
                reminder.Due = DateTime.Now.AddDays(project.MaxNoteIntervalInDaysBeforeReminder);
                RavenSession.Store(reminder);
            }
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