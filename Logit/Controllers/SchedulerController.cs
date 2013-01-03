using Logit.Business;
using Logit.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logit.Controllers
{
    public class SchedulerController : RavenBaseController
    {
        //
        // GET: /Scheduler/

        public ActionResult Index()
        {
            var schedule = RavenSession.Load<ScheduleSettings>("scheduleSettings/1");
            var data = new List<EmailReminderData>();

            int delayHours = 4;

            if (schedule == null)
            {
                schedule = new ScheduleSettings { Id = "scheduleSettings/1" }; RavenSession.Store(schedule); ViewBag.Message = "This is the first run!";
                ViewBag.Error="First run, creating session";
                return View();
            } 
            
            
            if ((DateTime.Now - schedule.LastRun).TotalHours > delayHours)
            {
                try
                {
                    var logic = new ReminderHelper(RavenSession, Server.MapPath("/Views/Templates/"));

                    var reminders = logic.GetAllRemindersDueBefore(DateTime.Now);
                    var msgs = logic.ComposeMessages(reminders);
                    ViewBag.Messages = msgs;
                    var emails = logic.SendEmails(msgs);
                    ViewBag.SentMessages = emails;
                    logic.UpdateMsgCount(reminders);

                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }

                    ViewBag.Message = "Last run: " + schedule.LastRun;
            
                if (data.Count > 0)
                {
                    string emailText = GetEmailText(data);
                    RavenSession.Store(new TempTextEmail { Text = emailText });
                }
                schedule.LastNumberOfNotifications = data.Count;
                schedule.LastRun = DateTime.Now;
                RavenSession.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Scheduler will not run before" + (delayHours-(DateTime.Now - schedule.LastRun).TotalHours).ToString() + " hours";
            }
            return View(data);
        }

        private string GetEmailText(List<EmailReminderData> data)
        {
            var text = "";
            foreach (var item in data)
            {
                foreach (var p in item.Reminders)
                {
                    text = string.Format("Name: {0}, Title {1}, LastNote {2}, No {3} --- \\n", p.FullName, p.ProjectTitle, p.LastNoteText, p.RemindersSent);
                    
                }
                
            }
            return text;

        }

    }
}
