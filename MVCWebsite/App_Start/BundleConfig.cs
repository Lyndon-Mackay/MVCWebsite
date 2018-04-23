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
            bundles.Add(new ScriptBundle("~/bundles/highlight").Include(
                "~/Scripts/jquery.mark.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/dateController").Include(
                "~/Scripts/Content/Controller/Date.js"));
            bundles.Add(new ScriptBundle("~/bundles/generalController").Include(
                "~/Scripts/Content/Controller/General.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/stylesheet.css"));
        }
    }
}