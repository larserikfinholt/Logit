using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    [Authorize]
    public class ProjectController : ApiController
    {
        private ProjectContext db = new ProjectContext();

        // GET api/TodoList
        public IEnumerable<ProjectDto> GetProjects()
        {
            return db.Projects.Include("Notes")
                .Where(u => u.UserId == User.Identity.Name)
                .OrderByDescending(u => u.ProjectId)
                .AsEnumerable()
                .Select(todoList => new ProjectDto(todoList));
        }

        // GET api/TodoList/5
        public ProjectDto GetProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if (project.UserId != User.Identity.Name)
            {
                // Trying to modify a record that does not belong to the user
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            return new ProjectDto(project);
        }

        // PUT api/TodoList/5
        public HttpResponseMessage PutProject(int id, ProjectDto projectDto)
        {
            if (ModelState.IsValid && id == projectDto.ProjectId)
            {
                Project todoList = projectDto.ToEntity();
                if (db.Entry(todoList).Entity.UserId != User.Identity.Name)
                {
                    // Trying to modify a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                db.Entry(todoList).State = EntityState.Modified;

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

        // POST api/TodoList
        public HttpResponseMessage PostProject(ProjectDto projectDto)
        {
            if (ModelState.IsValid)
            {
                projectDto.UserId = User.Identity.Name;
                Project todoList = projectDto.ToEntity();
                db.Projects.Add(todoList);
                db.SaveChanges();
                projectDto.ProjectId = todoList.ProjectId;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, projectDto);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = projectDto.ProjectId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/TodoList/5
        public HttpResponseMessage DeleteProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (db.Entry(project).Entity.UserId != User.Identity.Name)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            ProjectDto projectDto = new ProjectDto(project);
            db.Projects.Remove(project);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, projectDto);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}