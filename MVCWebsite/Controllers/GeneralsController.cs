using MVCWebsite.Models;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Collections.Generic;
using PagedList;

namespace MVCWebsite.Controllers
{
    public class GeneralsController : Controller
    {
        GenDBContext db = new GenDBContext();
        // GET: Generals
        public ActionResult Index(int? page, int pageSize = 25, string SearchString = "", string column = "ID", string sort = "asc",
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
                    MethodCallExpression containsInvoke = CreateInvoke(parameter, containsInfo, searchArgument, "Country");
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                {
                    Expression e = conditions.Aggregate<Expression>(
                        (combinedExpression, next) => combinedExpression = Expression.OrElse(combinedExpression, next));
                    while(e.CanReduce)
                    {
                        e = e.Reduce();
                    }
                    var vList = generals.Where(Expression.Lambda<Func<General, bool>>(e, parameter));
                    return PagedView(vList.ToList(), page, pageSize);
                }



            }
            return PagedView(db.Generals.ToList(), page, pageSize);
        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="parameter">what database item property is to be called</param>
        /// <param name="containsInfo">methodinfo intended to be the contains method , 
        /// but could be any method with one argument for a string</param>
        /// <param name="searchArgument">The argument used in the method e.g the search</param>
        /// <param name="propertyName">which property to call</param>
        /// <returns>an expression for method call intended to be used in a linq where statement</returns>
        [NonAction]
        private MethodCallExpression CreateInvoke(ParameterExpression parameter, MethodInfo containsInfo, ConstantExpression searchArgument, string propertyName)
        {
            MemberExpression searchproperty = Expression.Property(parameter, propertyName);

            MethodCallExpression containsInvoke = Expression.Call(searchproperty, containsInfo, searchArgument);
            return containsInvoke;
        }
        /// <summary>
        /// This method is used to create a view that takes paging into account
        /// </summary>
        /// <param name="vList">the list of items to display</param>
        /// <param name="page">optional page number defaults to 1</param>
        /// <param name="pageSize">number of items per page defaults to 25</param>
        /// <returns>The view to be displayed</returns>
        [NonAction]
        private ViewResult PagedView(List<General> vList, int? page, int pageSize = 25)
        {
            int pageNumber = (page ?? 1);
            return View(vList.ToPagedList(pageNumber, pageSize));

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