using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCWebsite.Controllers;
using MVCWebsite.Models;
using System.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;

namespace MVCWebsite.Controllers.Tests
{
    [TestClass()]
    public class GeneralsControllerTests
    {
        //TODO refactor the code 
        GenDBContext db = new GenDBContext();
        [TestMethod()]
        public void IndexTest()
        {
            TestDefault();
            TestChangedSort();
            TestSearchAll();
            TestNonExistentString();
            TestSortingAndSearch();
        }
        [TestMethod()]
        public void IDTest()
        {
            GeneralsController genController = new GeneralsController();
            Random gen = new Random();
            List<General> genList = db.Generals.ToList();
            int testID = gen.Next(genList.Count);
            ViewResult v = genController.Details(genList[testID].ID) as ViewResult;
            General g = v.Model as General;
            Assert.AreEqual(g, genList[testID], "Correct id is not being returned");

            //null check 
            genController = new GeneralsController();
            HttpStatusCodeResult correctStatus = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ActionResult details = genController.Details(null);
            HttpStatusCodeResult resultStatus = genController.Details(null) is HttpStatusCodeResult ? details as HttpStatusCodeResult : null;
            if (resultStatus == null)
            {
                Assert.Fail("not responding to nulls correctly");
            }
            else {
                Assert.AreEqual(correctStatus.StatusCode
                    , resultStatus.StatusCode, "Null check wrong status code");
            }

            //null general check
            genController = new GeneralsController();
            var nullGendetails = genController.Details(-1);
            var nullGenDetailsResult = nullGendetails is HttpNotFoundResult? nullGendetails as HttpNotFoundResult:null;
            if (nullGenDetailsResult == null)
            {
                Assert.Fail("did not handle non existent general properly");
            }
            else
            {

                var r = new HttpNotFoundResult();
                Assert.AreEqual(nullGenDetailsResult.StatusCode , r.StatusCode, "Did not handle non existent general");
            }
        }
        /// <summary>
        /// Tests on empty lists
        /// </summary>
        /// 
        private void TestDefault()
        {
            IQueryable<General> correctQuery;
            ViewResult view;
            CreateBaseQueryAndView("asc",out correctQuery, out view);
            List<General> correctResult = correctQuery.ToList();
            List<General> resultList = (List<General>)view.ViewData.Model;
            CollectionAssert.AreEqual(correctResult, resultList, "The Default list is not correct ");
        }
        /// <summary>
        /// Test for correctness on descended sort
        /// </summary>
        private void TestChangedSort()
        {
            IQueryable<General> correctQuery;
            ViewResult view;
            CreateBaseQueryAndView("desc", out correctQuery, out view);
            List<General> correctResult = correctQuery.ToList();
            List<General> resultList = (List<General>)view.ViewData.Model;
            CollectionAssert.AreEqual(correctResult, resultList, "The descending list is not correct ");
        }
        /// <summary>
        /// Test that searching for all is correct
        /// </summary>
        private void TestSearchAll()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            string search = "Barca";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search,SearchName:  true,SearchCountry:  true,SearchComments:  true) as ViewResult;

            List<General> resultList = (List<General>)view.ViewData.Model;
            Assert.IsTrue(resultList.All(x =>
            x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            General g = resultList.Find(x => !(x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            if(g != null)
            {
                Assert.Fail(g.Name);
            }
            CollectionAssert.AreEqual(correctResult, resultList, "The searchall list is not correct ");
        }
        /// <summary>
        /// Test that a string that is very likely to not be in the database 
        /// is in fact not in the database
        /// </summary>
        private void TestNonExistentString()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            //should not be in the database
            string search = "l;kl;jop";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model; ;
            //if true may need a new string
            Assert.IsTrue(correctResult.Count == 0,"string is now part of the databse found "+ correctResult.Count +"result");
            CollectionAssert.AreEqual(correctResult, resultList, "The SearchNonexistent list is not correct ");
        }
        /// <summary>
        /// test sorting works with search
        /// </summary>
        private void TestSortingAndSearch()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "desc");
            //TODO Should sanity check for proper characters to be done on public holiday or a weekend
            string search = "l;kl;jop";
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(sort:  "desc",SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model; ;
            //if true may need a new string
            Assert.IsTrue(correctResult.Count == 0, "string is now part of the databse found " + correctResult.Count + "result");
            CollectionAssert.AreEqual(correctResult, resultList, "The search while sorting list is not correct ");

        }
        /// <summary>
        /// test the search only checks the appropriate columns
        /// </summary>
        private void TestOnlyMatchName()
        {
            GeneralsController genController = new GeneralsController();
            var defaultQuery = db.Generals.OrderBy("ID " + "asc");
            string search = "Barca";
            defaultQuery = defaultQuery.Where("Name.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search, SearchName: true) as ViewResult;

            List<General> resultList = (List<General>)view.ViewData.Model;
            Assert.IsTrue(resultList.All(x =>
            x.Name.Contains(search)));
            CollectionAssert.AreEqual(correctResult, resultList, "The Search only name" +
                " list is not correct ");

        }

        /// <summary>
        /// Creates a basic query with a basic view common to most of the tests
        /// </summary>
        /// <param name="sort">string "asc" or desc</param>
        /// <param name="defaultQuery">The query asking for a list with the default sort</param>
        /// <param name="view">The viw returned by an iniltialised controller</param>
        private void CreateBaseQueryAndView(string sort, out IQueryable<General> defaultQuery, out ViewResult view)
        {
            GeneralsController genController = new GeneralsController();
            defaultQuery = db.Generals.OrderBy("ID " + sort);
            view = genController.Index() as ViewResult;
        }

    }
}