using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Logit.Models;

using System.IO;
using System.Net.Mail;
using System.Net;
using RazorEngine;

namespace Logit.Business
{
    public class ReminderHelper
    {
        public IDocumentSession RavenSession { get; private set; }
        private string TemplateRootPath;

        public ReminderHelper(IDocumentSession session, string templateRootPath)
        {
            RavenSession = session;
            TemplateRootPath = templateRootPath;
        }

        public IList<Reminder> GetAllRemindersDueBefore(DateTime due)
        {
            return RavenSession.Query<Reminder>().Where(x => x.Due < due && x.RemindersSent < 3).ToList();

        }


        public List<EmailReminderData> ComposeMessages(IList<Reminder> reminders)
        {
            var q = reminders.GroupBy(x => x.UserId);
            var retVal = new List<EmailReminderData>();
            foreach (var item in q)
            {
                var u = RavenSession.Load<User>(item.First().UserId);
                var toAdd = new EmailReminderData { Reminders = new List<Reminder>(), FullName = u.FullName, Email = u.Email };
                foreach (var reminder in item)
                {
                    toAdd.Reminders.Add(reminder);
                }
                //var template = Template.Compile(File.ReadAllText(TemplateRootPath + "Reminder.cshtml"));
                //toAdd.HtmlBodyString = template.Render( toAdd);
                toAdd.HtmlBodyString = Razor.Parse(File.ReadAllText(TemplateRootPath + "Reminder.cshtml"), toAdd);
                retVal.Add(toAdd);
            }

            return retVal;
        }

        public void UpdateMsgCount(List<EmailReminderData> result)
        {
            throw new NotImplementedException();
        }

        public void UpdateMsgCount(IList<Reminder> reminders)
        {
            foreach (var reminder in reminders)
            {
                var toIncrease = RavenSession.Load<Reminder>(reminder.Id);
                toIncrease.RemindersSent++;
            }
        }

        public int SendEmails(List<EmailReminderData> msgs)
        {
            var i = 0;
            foreach (var user in msgs)
            {
                var fromAddress = new MailAddress("reminder.goaly@gmail.com", "Logit");
                var toAddress = new MailAddress(user.Email, user.FullName);
                const string fromPassword = "logitlogit";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Reminder from Logit!",
                    Body = user.HtmlBodyString,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                i++;
            }
            return i;

        }
    }
}