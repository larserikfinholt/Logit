using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logit.Models
{



    public class Project
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public int MaxNoteIntervalInDaysBeforeReminder { get; set; }
    }
    public class Note
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpated { get; set; }
        public string Text { get; set; }
    }

    public class Reminder
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string LastNoteText { get; set; }
        public DateTime Due { get; set; }
        public int RemindersSent { get; set; }
    }
    public class EmailReminderData
    {
        public string FullName { get; set; }
        public string HtmlBodyString { get; set; }
        public List<Reminder> Reminders { get; set; }

        public string Email { get; set; }
    }
    public class ScheduleSettings
    {
        public string Id { get; set; }
        public DateTime LastRun { get; set; }
        public int LastNumberOfNotifications { get; set; }
        
    }
    public class TempTextEmail
    {
        public string Id { get; set; }
        public string Text { get; set; }
        
    }


}