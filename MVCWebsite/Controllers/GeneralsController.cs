using MVCWebsite.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCWebsite.Controllers
{
    public class GeneralsController : Controller
    {
        GenDBContext db = new GenDBContext();
        // GET: Generals
        public ActionResult Index(string column = "ID", string sort = "asc")
        {
            var generals = db.Generals.OrderBy(column + " " +sort);
            return View(generals.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            General general = db.Generals.Find(id);
            if(general == null)
            {
                return HttpNotFound();
            }
            return View(general);
        }
    }
}