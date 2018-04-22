using MVCWebsite.Models;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Collections.Generic;

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
                ParameterExpression parameter = Expression.Parameter(typeof(General), "g");

                MethodInfo containsInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                ConstantExpression searchArgument = Expression.Constant(SearchString, typeof(string));

                List<MethodCallExpression> conditions = new List<MethodCallExpression>();

                if (SearchCountry)
                {
                    MethodCallExpression containsInvoke = CreateInvoke(parameter, containsInfo, searchArgument,"Country");

                    conditions.Add(containsInvoke);

                }
                if (SearchName)
                {
                    MethodCallExpression containsInvoke = CreateInvoke(parameter, containsInfo, searchArgument, "Name");
                    conditions.Add(containsInvoke);
                }
                if (SearchComments)
                {
                    MethodCallExpression containsInvoke = CreateInvoke(parameter, containsInfo, searchArgument, "Comments");

                    conditions.Add(containsInvoke);
                }
                if (conditions.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else if (conditions.Count == 1)
                {
                    var vlist = generals.Where(Expression.Lambda<Func<General, bool>>(conditions[0], parameter));
                    return View(vlist.ToList());
                }
                else
                {
                    Expression e = Expression.OrElse(conditions[0], conditions[1]);
                    //skip first two as they are already done
                    for (int i = 2; i < conditions.Count; i++)
                    {
                        e = Expression.OrElse(e, conditions[i]);
                    }
                    System.Diagnostics.Debug.WriteLine(e.Reduce());
                    var vlist = generals.Where(Expression.Lambda<Func<General, bool>>(e, parameter));
                    return View(vlist.ToList());
                }
            }
            return View(generals.ToList());
        }
        [NonAction]
        private static MethodCallExpression CreateInvoke(ParameterExpression parameter, MethodInfo containsInfo, ConstantExpression searchArgument,string propertyName)
        {
            MemberExpression searchproperty = Expression.Property(parameter, propertyName);

            MethodCallExpression containsInvoke = Expression.Call(searchproperty, containsInfo, searchArgument);
            return containsInvoke;
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