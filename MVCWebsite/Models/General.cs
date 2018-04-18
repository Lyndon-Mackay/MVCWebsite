using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCWebsite.Models
{
    public class General
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Comments { get; set; }

        public string Wiki_link { get; set; }


    }
    public class GenDBContext : DbContext {
        public GenDBContext() { }
        public DbSet<General> Generals { get; set; }
    }
}