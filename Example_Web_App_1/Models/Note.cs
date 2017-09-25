using System;
using System.Collections.Generic;

namespace Example_Web_App_1.Models
{
    public partial class Note
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public DateTime CreationTime { get; set; }
        public string Content { get; set; }
        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}
