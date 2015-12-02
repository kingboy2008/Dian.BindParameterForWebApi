using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class UserAuthVm
    {
        public ulong Id { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }
    }

    public class UserAuthListVm
    {
        public ulong Id { get; set; }

        public List<ulong> Account { get; set; }

        public string Password { get; set; }
    }
}