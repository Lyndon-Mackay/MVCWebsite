using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace MVCWebsite.Models
{
    public class Date
    {
        public int ID { get; set; }

        public DateTime Time { get; set; }
    }
    public class DateDBContext : DbContext
    {
        public DateDBContext() { }
        public DbSet<Date> Dates { get; set; }
    }

}