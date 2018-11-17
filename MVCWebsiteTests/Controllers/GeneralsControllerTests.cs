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
using System.Data.Entity;
using Moq;

namespace MVCWebsite.Controllers.Tests
{
    [TestClass()]
    public class GeneralsControllerTests
    {
        //TODO refactor the code 
        GenDBContext db = new GenDBContext();

        const string ASCENDING_SORT = "asc";
        const string DESCENDING_SORT = "desc";
        //persistent controller as even with state it should be correct
        GeneralsController genController = new GeneralsController();
        [TestMethod()]
        public void MoqTest()
        {
            #region arrange
            var data = new List<General>
            {
                new General{ Name = "Hannibal Barca",Country = "Carthage",Comments ="Carthaginian general during the second Punic war." ,Wiki_link = "https://en.wikipedia.org/wiki/Hannibal"},
                new General{ Name = "Pyhrrus",Country = "Epirus",Comments ="Fought in the Pyrrhic war, the term Pyrrhic victory is named after him." ,Wiki_link = "https://en.wikipedia.org/wiki/Pyrrhus_of_Epirus"},
                new General{ Name = "Hamilcar Barca",Country = "Carthage",Comments ="Father of Hannibal Barca was a general of Carthage during the first Punic war gained the epithet Barca meaning lightning." ,Wiki_link = "https://en.wikipedia.org/wiki/Hamilcar_Barca"},
                new General{ Name = "Flavius Aetius",Country = "Western Roman Empire",Comments ="Best known for his victory at the Battle of the Catalaunian Plains against Atilla the Hun." ,Wiki_link = "https://en.wikipedia.org/wiki/Flavius_Aetius"},
                new General{ Name = "Subatai",Country = "Mongolia", Comments = "Mongolian general conquered more territory then any other general in history.", Wiki_link = "https://en.wikipedia.org/wiki/Subutai" },
                new General {Name = "Khalid ibn al-Walid",Country = "Rashidun Caliphate",Comments = "Known for his Victories against the Byzantine and Sassanian Empires. He was undefeated.",Wiki_link = "https://en.wikipedia.org/wiki/Khalid_ibn_al-Walid" },
                new General { Name = "Scipio Africanus",Country = "Rome",Comments = "Best known for his role in defeating Hannibal Barca during the second Punic war. Prior to that he campaigned against Carthage in modern day Spain and Portugal.", Wiki_link = "https://en.wikipedia.org/wiki/Scipio_Africanus" },
                new General {Name = "Sulla",Country= "Rome",Comments = "Best known for his dictatorship over Rome. He also lead in wars against Mithridates and the Socii.", Wiki_link = "https://en.wikipedia.org/wiki/Sulla" }
            }.AsQueryable();
            Mock<DbSet<General>> mockSet = new Mock<DbSet<General>>();

            mockSet.As<IQueryable<General>>().Setup(x => x.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<General>>().Setup(x => x.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<General>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<General>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            GeneralsController view = new GeneralsController
            {
                GenDbGenerals = mockSet.Object
            };
            #endregion
            #region access
            var resultView = view.Index();
            var result = view.ViewData.Model as List<General>;

            #endregion
            #region assert
            var correctResult = data.OrderBy("ID " + ASCENDING_SORT).ToList();
            CollectionAssert.AreEqual(correctResult, result, "Moq test failed");
            #endregion
        }
        [TestMethod()]
        public void IDTest()
        {

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
            else
            {
                Assert.AreEqual(correctStatus.StatusCode
                    , resultStatus.StatusCode, "Null check wrong status code");
            }

            //null general check
            genController = new GeneralsController();
            var nullGendetails = genController.Details(-1);
            var nullGenDetailsResult = nullGendetails is HttpNotFoundResult ? nullGendetails as HttpNotFoundResult : null;
            if (nullGenDetailsResult == null)
            {
                Assert.Fail("did not handle non existent general properly");
            }
            else
            {

                var r = new HttpNotFoundResult();
                Assert.AreEqual(nullGenDetailsResult.StatusCode, r.StatusCode, "Did not handle non existent general");
            }
        }

        /// <summary>
        /// Tests on empty lists
        /// </summary>
        /// 
        [TestMethod()]
        public void TestDefault()
        {
            //arrange
            IQueryable<General> correctQuery;
            ViewResult view;
            CreateBaseQueryAndView(ASCENDING_SORT, out correctQuery, out view);
            //act
            List<General> correctResult = correctQuery.ToList();
            List<General> resultList = (List<General>)view.ViewData.Model;
            //assert
            CollectionAssert.AreEqual(correctResult, resultList, "The Default list is not correct ");
        }
        /// <summary>
        /// Test for correctness on descended sort
        /// </summary>
        [TestMethod()]
        public void TestChangedSort()
        {
            //arrange
            IQueryable<General> correctQuery;
            ViewResult view;
            CreateBaseQueryAndView(DESCENDING_SORT, out correctQuery, out view);

            //act
            List<General> resultList = (List<General>)view.ViewData.Model;
            List<General> correctResult = correctQuery.ToList();

            //assert
            CollectionAssert.AreEqual(correctResult, resultList, "The descending list is not correct ");
        }
        /// <summary>
        /// Test that searching for all is correct
        /// </summary>
        [TestMethod()]
        public void TestSearchAll()
        {
            //arrange
            const string search = "Barca";
            List<General> correctResult = FullSearch(search, ASCENDING_SORT);
            var view = genController.Index(SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            //act 
            List<General> resultList = (List<General>)view.ViewData.Model;

            //assert
            Assert.IsTrue(resultList.All(x =>
            x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            General g = resultList.Find(x => !(x.Country.Contains(search) || x.Name.Contains(search) || x.Comments.Contains(search)));
            if (g != null)
            {
                Assert.Fail(g.Name + "incorrect general found");
            }
            CollectionAssert.AreEqual(correctResult, resultList, "The searchall list is not correct ");
        }


        /// <summary>
        /// Test that a string that is very likely to not be in the database 
        /// is in fact not in the database
        /// </summary>
        [TestMethod()]
        public void TestNonExistentString()
        {
            //arrange
            //should not be in the database
            const string search = "l;kl;jop";
            List<General> correctResult = FullSearch(search, ASCENDING_SORT);

            //act
            var view = genController.Index(SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model;

            //assert
            //if true may need a new string
            Assert.IsTrue(correctResult.Count == 0, "string is now part of the databse found " + correctResult.Count + "result");
            CollectionAssert.AreEqual(correctResult, resultList, "The SearchNonexistent list is not correct ");
        }
        /// <summary>
        /// test sorting works with search
        /// </summary>
        [TestMethod()]
        public void TestSortingAndSearch()
        {
            //arrange
            //should have valid results
            string search = "rom";
            List<General> correctResult = FullSearch(search, DESCENDING_SORT);
            //act
            var view = genController.Index(sort: DESCENDING_SORT, SearchString: search, SearchName: true, SearchCountry: true, SearchComments: true) as ViewResult;
            List<General> resultList = (List<General>)view.ViewData.Model;
            //Assert
            //if true may need a new string
            CollectionAssert.AreEqual(correctResult, resultList, "The descending sort does not work when searched");

        }
        /// <summary>
        /// test the search only checks the appropriate columns
        /// </summary>
        /// 
        [TestMethod()]
        public void TestOnlyMatchName()
        {
            //arrange
            var defaultQuery = db.Generals.OrderBy("ID " + ASCENDING_SORT);
            const string search = "Barca";
            defaultQuery = defaultQuery.Where("Name.contains(@0)", search);
            List<General> correctResult = defaultQuery.ToList();
            var view = genController.Index(SearchString: search, SearchName: true) as ViewResult;

            //act
            List<General> resultList = (List<General>)view.ViewData.Model;

            //assert
            Assert.IsTrue(resultList.All(x =>
            x.Name.Contains(search)));
            CollectionAssert.AreEqual(correctResult, resultList, "The Search only name" +
                " list is not correct ");

        }
        /**
         * 
         */

        /// <summary>
        /// Creates a basic query with a basic view common to most of the tests
        /// </summary>
        /// <param name="sort">string "asc" or desc</param>
        /// <param name="defaultQuery">The query asking for a list with the default sort</param>
        /// <param name="view">The viw returned by an iniltialised controller</param>
        private void CreateBaseQueryAndView(string sort, out IQueryable<General> defaultQuery, out ViewResult view)
        {
            defaultQuery = db.Generals.OrderBy("ID " + sort);
            view = genController.Index(sort: sort) as ViewResult;
        }

        private List<General> FullSearch(string search, string sort)
        {
            IQueryable<General> defaultQuery;
            defaultQuery = db.Generals.OrderBy("ID " + sort);
            defaultQuery = defaultQuery.Where("Name.contains(@0) or Country.contains(@0) or Comments.contains(@0)", search);
            return defaultQuery.ToList();
        }

    }
}