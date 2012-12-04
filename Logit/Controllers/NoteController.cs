using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    [Authorize]
    public class NoteController : ApiController
    {
        private ProjectContext db = new ProjectContext();

        // PUT api/Todo/5
        public HttpResponseMessage PutNoteItem(int id, NoteItemDto noteItemDto)
        {
            if (ModelState.IsValid && id == noteItemDto.TodoItemId)
            {
                NoteItem noteItem = noteItemDto.ToEntity();
                Project project = db.Projects.Find(noteItem.ProjectId);
                if (project == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (project.UserId != User.Identity.Name)
                {
                    // Trying to modify a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                // Need to detach to avoid duplicate primary key exception when SaveChanges is called
                db.Entry(project).State = EntityState.Detached;
                db.Entry(noteItem).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/Todo
        public HttpResponseMessage PostNoteItem(NoteItemDto noteItemDto)
        {
            if (ModelState.IsValid)
            {
                Project project = db.Projects.Find(noteItemDto.TodoListId);
                if (project == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (project.UserId != User.Identity.Name)
                {
                    // Trying to add a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                NoteItem todoItem = noteItemDto.ToEntity();

                // Need to detach to avoid loop reference exception during JSON serialization
                db.Entry(project).State = EntityState.Detached;
                db.NoteItems.Add(todoItem);
                db.SaveChanges();
                noteItemDto.TodoItemId = todoItem.NoteItemId;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, noteItemDto);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = noteItemDto.TodoItemId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Todo/5
        public HttpResponseMessage DeleteNoteItem(int id)
        {
            NoteItem noteItem = db.NoteItems.Find(id);
            if (noteItem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (db.Entry(noteItem.Project).Entity.UserId != User.Identity.Name)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            NoteItemDto todoItemDto = new NoteItemDto(noteItem);
            db.NoteItems.Remove(noteItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoItemDto);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}