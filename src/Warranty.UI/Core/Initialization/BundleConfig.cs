namespace Warranty.UI.Core.Initialization
{
    using System.Web.Optimization;
    using Bundling;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            var lessBundle = new StyleBundle("~/css/less/main").Include(
                "~/css/less/bootstrap.less",
                "~/css/less/bootstrap-custom.less",
                "~/css/less/toastr.less",
                "~/css/site.less");
            lessBundle.Transforms.Add(new LessBundleTransform());
            lessBundle.Transforms.Add(new CssMinify());

            var cssBundle = new StyleBundle("~/css/extra").Include(
                "~/css/font.css",
                "~/css/bootstrap-editable.css");
            cssBundle.Transforms.Add(new CssMinify());

            bundles.Add(lessBundle);
            bundles.Add(cssBundle);
        }
    }
}
