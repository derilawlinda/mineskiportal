using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineskiPortal.Models
{
    public class GameClicks
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int TodayClicks { get; set; }
        public int WeekClicks { get; set; }
        public int MonthClicks { get; set; }
        public int TotalClicks { get; set; }

    }
}
