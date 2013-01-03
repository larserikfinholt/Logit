using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logit.Business;
using Raven.Client.Embedded;
using Raven.Client;
using Logit.Models;
using Logit.Services;
using ServiceStack.ServiceInterface.Testing;
using System.Collections.Generic;

namespace Logit.Tests
{
    [TestClass]
    public class ReminderTests
    {
        public IDocumentSession RavenSession { get; set; }
        public string templateRootPath = @"C:\Projects\Logit\Logit\Views\Templates\";

        [TestInitialize]
        public void SetUp()
        {
            var _store = new EmbeddableDocumentStore
            {
                RunInMemory = true,
            };

            _store.Initialize();

            RavenSession = _store.OpenSession();

            RavenSession.Store(new User { Id = "users/99", Email = "lars.erik.finholt@gmail.com", UserName = "lef", Enabled = true, FullName = "Lars Erik Finhot" });
            RavenSession.Store(new User { Id = "users/98", Email = "lars.erik.finholt@gmail.com", UserName = "jalla", Enabled = true, FullName = "Dølle Duck" });
            RavenSession.Store(new Project { Owner = "users/99", Title = "Project99", Id="projects/99", MaxNoteIntervalInDaysBeforeReminder=7, Description="Description text" });
            RavenSession.SaveChanges();
        }

        [TestMethod]
        public void verify_loads_reminders()
        {
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 20), RemindersSent = 0, LastNoteText = "Created 20.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 19), RemindersSent = 1, LastNoteText = "Created 19.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 18), RemindersSent = 2, LastNoteText = "Created 18.10", ProjectId = "projects/99" });
            RavenSession.SaveChanges();

            var target = new ReminderHelper(RavenSession, templateRootPath);
            var a = target.GetAllRemindersDueBefore(new DateTime(2012, 10, 17));
            Assert.AreEqual(0,a.Count );

            a = target.GetAllRemindersDueBefore(new DateTime(2012, 10, 21));
            Assert.AreEqual(3, a.Count); 
        }

        [TestMethod]
        public void verify_compose_message_with_all_reminders_for_one_users()
        {
            var reminders = new List<Reminder>();
            reminders.Add(new  Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 20), RemindersSent = 0, LastNoteText = "Created 20.10", ProjectId = "projects/99", ProjectTitle="Project99" });
            reminders.Add(new Reminder { UserId = "users/98", Due = new DateTime(2012, 10, 19), RemindersSent = 0, LastNoteText = "Created 19.10", ProjectId = "projects/98", ProjectTitle = "Project99" });
            reminders.Add(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 18), RemindersSent = 0, LastNoteText = "Created 18.10", ProjectId = "projects/99", ProjectTitle = "Project99" });

            var target = new ReminderHelper(RavenSession,templateRootPath);
            var result = target.ComposeMessages(reminders);

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].Reminders.Count, 2);
            Assert.AreEqual(result[1].Reminders.Count, 1);
            Assert.IsTrue(result[0].HtmlBodyString.Contains("Project99"));
        }

        [TestMethod]
        public void verify_increase_sentcount()
        {


            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 20), RemindersSent = 0, LastNoteText = "Created 20.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 19), RemindersSent = 1, LastNoteText = "Created 19.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 18), RemindersSent = 2, LastNoteText = "Created 18.10", ProjectId = "projects/99" });
            RavenSession.SaveChanges();

            var target = new ReminderHelper(RavenSession, templateRootPath);
            var result = target.GetAllRemindersDueBefore(new DateTime(2012, 10, 20));
            Assert.AreEqual(result.Count, 2);
            target.UpdateMsgCount(result);
            RavenSession.SaveChanges();
            result = target.GetAllRemindersDueBefore(new DateTime(2012, 10, 20));
            Assert.AreEqual(result.Count, 1);
        }


        [TestMethod]
        public void verify_sends_email()
        {
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 20), RemindersSent = 0, LastNoteText = "Created 20.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 19), RemindersSent = 1, LastNoteText = "Created 19.10", ProjectId = "projects/99" });
            RavenSession.Store(new Reminder { UserId = "users/99", Due = new DateTime(2012, 10, 18), RemindersSent = 2, LastNoteText = "Created 18.10", ProjectId = "projects/99" });
            RavenSession.SaveChanges();

            var target = new ReminderHelper(RavenSession, templateRootPath);
            var reminders = target.GetAllRemindersDueBefore(new DateTime(2012, 10, 20));
            var msgs = target.ComposeMessages(reminders);
           // var emails = target.SendEmails(msgs);
            
        }

        [TestMethod]
        public void verify_adds_reminder_if_project_is_configured_with_reminders()
        {

            RavenSession.Store(new Project { Owner = "users/99", Title = "Project98", Id = "projects/98", MaxNoteIntervalInDaysBeforeReminder = 7, Description = "Description text" });
            RavenSession.SaveChanges();
            var target = NoteService.CreateNoteService(RavenSession, RavenSession.Load<User>("users/99"));
            target.RequestContext = new MockRequestContext2();

            var help = new ReminderHelper(RavenSession, templateRootPath);
            var a = help.GetAllRemindersDueBefore(DateTime.Now);
            Assert.AreEqual(0, a.Count);

            target.OnPost(new Note { ProjectId = "projects/98", Text = "first on 98" });

            var n = RavenSession.Load<Note>("notes/1");
            Assert.AreEqual(n.ProjectId, "projects/98");


            var r = RavenSession.Load<Reminder>("reminders/1");
            Assert.AreEqual(r.ProjectId, "projects/98");
            Assert.AreEqual(r.Due.ToShortDateString(), DateTime.Now.AddDays(7).ToShortDateString());


            //a = (new ReminderHelper(RavenSession)).GetAllRemindersDueBefore(DateTime.Now.AddDays(9));
            //Assert.AreEqual(1, a.Count);

            
        }

    }
}
