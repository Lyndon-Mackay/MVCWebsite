using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace MVCWebsite.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/controller").Include(
                "~/Scripts/Controller/*.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/stylesheet.css"));
        }
    }
}