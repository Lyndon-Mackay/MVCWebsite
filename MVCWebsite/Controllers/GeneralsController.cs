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
                /*
                * Creating a custom expression so the database is only queried once
                * cannot just call .where as multiple wheres are anded rather then ored
                */ 
                ParameterExpression parameter = Expression.Parameter(typeof(General), "g");

                MethodInfo containsInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                ConstantExpression searchArgument = Expression.Constant(SearchString, typeof(string));

                List<MethodCallExpression> conditions = new List<MethodCallExpression>();

                /*checking if the search fields are checked and adding it towards the search
                 * Would prefer this conditions in a dictionary for scalibility reasons, but no idea how to do that
                 * with HTML get
                 */ 
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
                // we should not be searching in no columns
                if (conditions.Count == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                else
                {
                    Expression e = conditions.Aggregate<Expression>(
                        (combinedExpression, next) => combinedExpression = Expression.OrElse(combinedExpression, next));
                    //probably not reducable but worth having a look could save significant time in querying 
                    while(e.CanReduce)
                    {
                        e = e.Reduce();
                    }
                    //One query called!!!
                    var vList = generals.Where(Expression.Lambda<Func<General, bool>>(e, parameter));
                    /*
                     * Done as parallel since is embarrsiningly parrallel plus the overhead will be neglible 
                    *on a small query anyway
                    */ 
                    return View(vList.AsParallel().ToList());
                }



            }
            return View(generals.ToList());
        }
        /// <summary>
        /// Used to create a call for a method that uses the search argument plus other built up expressions
        /// the main point of this method is have common code save for the property being called.
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