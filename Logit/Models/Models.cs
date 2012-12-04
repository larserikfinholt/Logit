using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logit.Models
{



    public class Project
    {
        public string ProjectId { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
    }
    public class Note
    {
        public string NoteId { get; set; }
        public string ProjectId { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
    }

}