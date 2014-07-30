using System.Web.Optimization;
using Warranty.UI.Core.Initialization.Bundling;

namespace Warranty.UI.Core.Initialization
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // TODO: Uncomment once we have the bootstrap / css loaded in
            //var lessBundle = new StyleBundle("~/css/main").Include(
            //    "~/css/less/bootstrap.less",
            //    "~/css/less/responsive.less",
            //    "~/css/less/toastr.less",
            //    "~/css/less/liveEditor.less",
            //    "~/css/site.less");
            //lessBundle.Transforms.Add(new LessBundleTransform());
            //lessBundle.Transforms.Add(new CssMinify());

            //var cssBundle = new StyleBundle("~/css/extra").Include(
            //    "~/css/font.css",
            //    "~/css/common.css",
            //    "~/css/site-responsive.css",
            //    "~/css/datepicker.css",
            //    "~/css/datatables.css",
            //    "~/css/fullcalendar.css",
            //    "~/css/bootstrap-tagmanager.css",
            //    "~/css/colorpicker.css");
            //cssBundle.Transforms.Add(new CssMinify());

            //bundles.Add(lessBundle);
            //bundles.Add(cssBundle);

            //BundleTable.EnableOptimizations = true;
        }
    }
}