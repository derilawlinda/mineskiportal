using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineskiPortal.Models
{
    public class Events
    {
        public Int32 Id { get; set; }
        public string EventName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

    }
}
