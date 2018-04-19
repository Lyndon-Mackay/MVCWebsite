using MVCWebsite.Models;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MVCWebsite.Controllers
{
    public class GeneralsController : Controller
    {
        GenDBContext db = new GenDBContext();
        // GET: Generals
        public ActionResult Index(string SearchString = "", string column = "ID", string sort = "asc")
        {
            var generals = db.Generals.OrderBy(column + " " + sort);
             
            if (!string.IsNullOrEmpty(SearchString))
            {
                generals = generals.Where(g => g.Country.Contains(SearchString) || g.Name.Contains(SearchString));
            }
            return View(generals.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            General general = db.Generals.Find(id);
            if (general == null)
            {
                return HttpNotFound();
            }
            return View(general);
        }
    }
}