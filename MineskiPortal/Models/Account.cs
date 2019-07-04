using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineskiPortal.Models
{
    public class Account
    {
        public Int32 Id { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }       
    }
}
