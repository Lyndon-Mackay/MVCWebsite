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
        public ActionResult Index(string SearchString = "", string column = "ID", string sort = "asc",
            bool SearchCountry = false, bool SearchName = false, bool SearchComments = false)
        {
            var generals = db.Generals.OrderBy(column + " " + sort);

            if (!string.IsNullOrEmpty(SearchString))
            {
                if (SearchCountry)
                {
                    generals = generals.Where(g => g.Country.Contains(SearchString));
                }
                if(SearchName)
                {
                    generals = generals.Where(g => g.Name.Contains(SearchString));
                }
                if (SearchComments)
                {
                    generals = generals.Where(g => g.Comments.Contains(SearchString));
                }
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