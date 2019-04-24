using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.Models
{
    public class Email
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}