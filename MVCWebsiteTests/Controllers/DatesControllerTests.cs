using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCWebsite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using MVCWebsite.Models;
using System.Web.Mvc;
using System.Net;

namespace MVCWebsite.Controllers.Tests
{
    [TestClass()]
    public class DatesControllerTests
    {

        private DateDBContext db = new DateDBContext();

        /// <summary>
        /// Test asc sort works
        /// </summary>
        [TestMethod()]
        public void TestBasicSort()
        {
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;
            DatesController datesController = new DatesController();
            var view = datesController.Index("", 1, 25) as ViewResult;
            PagedList<Date> resultPage = view.Model as PagedList<Date>;
            List<Date> resultList = resultPage.ToList();
            List<Date> correctList = dates.ToPagedList(1, 25).ToList();
            CollectionAssert.AreEqual(resultList, correctList, "Basic sorting is not working correctly");
        }
        /// <summary>
        /// Test desc sort works
        /// </summary>
        [TestMethod()]
        public void TestDescSort()
        {
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;
            dates = dates.OrderByDescending(d => d.Time);
            DatesController datesController = new DatesController();
            var view = datesController.Index("desc", 1, 25) as ViewResult;
            PagedList<Date> resultPage = view.Model as PagedList<Date>;
            List<Date> resultList = resultPage.ToList();
            List<Date> correctList = dates.ToPagedList(1, 25).ToList();

            CollectionAssert.AreEqual(resultList, correctList, "Desc sorting is not working correctly");
        }
        //TODO test against pagination
        [TestMethod()]
        public void TestAddAndDelete()
        {
            DatesController datesController = new DatesController();
            Random random = new Random();
            int newId = 0;
            //get a unique id for a new date
            do
            {
                newId = random.Next();
            } while (db.Dates.Any(d => d.ID == newId));//while any id is the same as the newone
            //adding new time to database
            Date testDate = new Date(newId, DateTime.Now);
            datesController.Create(testDate);
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;
            Assert.IsTrue(dates.ToList().Any(d => d.ID == testDate.ID), "Date was not added to the database");

            datesController.DeleteConfirmed(testDate.ID);

            Assert.IsFalse(dates.ToList().Any(d => d.ID == testDate.ID), "Date was not removed from the database");
        }
        [TestMethod()]
        public void EditAndDeleteTest()
        {
            DatesController datesController = new DatesController();
            Random random = new Random();
            Date date;
            List<Date> dateList = db.Dates.ToList();
            date = dateList[random.Next(dateList.Count)];

            DateTime dateTime = DateTime.Now;
            date.Time = dateTime;
            var dates = from d in db.Dates
                        orderby d.Time
                        select d;

            datesController.Edit(date);

            Date actualDate = dates.ToList().Find(d => d.ID == date.ID);
            Assert.AreEqual(dateTime, actualDate.Time, "Time has not been edited correctly");
            ViewResult deleteView =  datesController.Delete(date.ID) as ViewResult;
            Date deleteDate = deleteView.Model as Date;
            Assert.AreEqual(dateTime, deleteDate.Time, "wrong time displayed on delete");

        }
        [TestMethod()]
        public void NullChecks()
        {
            DatesController datesController = new DatesController();
            HttpStatusCodeResult correctNullInvalid =new HttpStatusCodeResult(HttpStatusCode.NotFound);
            ActionResult editNull = datesController.Edit(-1);
            ActionResult deleteNull = datesController.Delete(-1);
            if(!(editNull is HttpStatusCodeResult))
            {
                Assert.Fail("Editing for an invalid id was not handled correctly");
            }
            if(!(deleteNull is HttpStatusCodeResult))
            {
                Assert.Fail("Delete for an invalid ID was not handled correctly");
            }
            HttpStatusCodeResult editNullInvalid = datesController.Edit(-1) as HttpStatusCodeResult;
            HttpStatusCodeResult deleteNullInvalid = datesController.Delete(-1) as HttpStatusCodeResult;
            Assert.AreEqual(correctNullInvalid.StatusCode, editNullInvalid.StatusCode , "wrong invalid code sent on edit");
            Assert.AreEqual(correctNullInvalid.StatusCode, deleteNullInvalid.StatusCode , "wrong invalid code sent on Delete");
        }
    }
}