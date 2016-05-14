using System.Web;
using System.Web.Optimization;

namespace ProStoneIMS2015
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunob").Include(
                "~/Scripts/jquery.unobtrusive*"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bsmodal-hash.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-reset.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/style.css",
                      "~/Content/style-responsive.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/adminjs").Include(
                        "~/Scripts/jquery.dcjqaccordion.2.7.js",
                        "~/Scripts/jquery.scrollTo.min.js",
                        "~/Scripts/jquery.nicescroll.js",
                        "~/Scripts/common-scripts.js"));

            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-reset.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/style.css",
                      "~/Content/style-responsive.css"));
        }
    }
}
