using Logit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logit.Controllers
{
    public class TemplatesController : Controller
    {
        //
        // GET: /Templates/

        public ActionResult Reminder()
        {
            var data = new EmailReminderData
            {
                FullName = "Jalla",
                Reminders = new List<Reminder> { 
                new Reminder { ProjectTitle="MyProject1", LastNoteText="Last note text", Due=DateTime.Now },
                new Reminder { ProjectTitle="MyProject2", LastNoteText="Last note text", Due=DateTime.Now }
            }
            };
            return View(data);
        }

    }
}
