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

    public class NoteService : RestServiceBase<Note>
    {
        private readonly List<Note> notes = new List<Note> { new Note { Text = "Text1", NoteId = "notes/1", ProjectId = "projects/1" }, new Note { Text = "Text2", NoteId = "notes/2", ProjectId = "projects/1" } };

        public override object OnGet(Note request)
        {
            if (string.IsNullOrEmpty(request.NoteId))
                return notes;

            return notes.First();
        }

        public override object OnPost(Note n)
        {
            var newNote = n;
            newNote.NoteId = "notes/4";
            newNote.Text = n.Text + "server - created";
            return new HttpResult(newNote)
            {
                StatusCode = HttpStatusCode.Created,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + newNote.NoteId}
                }
            };
        }

        public override object OnPut(Note note)
        {
            var updated = note;
            updated.Text = note.Text + "server - updated"; 
            return new HttpResult(updated)
            {
                StatusCode = HttpStatusCode.OK,
                Headers = {
                    { HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + note.NoteId}
                }
            };
        }

    }


}