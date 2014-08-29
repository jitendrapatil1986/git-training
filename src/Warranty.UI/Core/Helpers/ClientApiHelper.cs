namespace Warranty.UI.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Warranty.Core.Extensions;

    public class ClientApiHelper
    {
        public IEnumerable<ClientApi> GetUrls()
        {
            var urls= (from type in typeof(ApiController).Assembly.GetTypes()
                   where type.IsAssignableTo<System.Web.Mvc.Controller>() && !type.IsAbstract
                   let controller = type.Name.Replace("Controller", "")
                   let methods = from method in type.GetMethods()
                                 where !method.HasAttribute<System.Web.Mvc.HttpPostAttribute>() && method.IsPublic && method.ReturnType.IsAssignableTo<System.Web.Mvc.ActionResult>()
                                 select new ClientApiUrl { Action = method.Name, Controller = controller }
                   select new ClientApi { Name = controller, Methods = methods }).ToList();

            var apiUrls = typeof (ApiController).Assembly.GetTypes()
                                                .Where(x => x.IsAssignableTo<ApiController>() && !x.IsAbstract)
                                                .Select(x => new ClientApi
                                                                 {
                                                                     Name = x.Name.Replace("Controller", ""),
                                                                     Methods = x.GetMethods()
                                                                                .Where(method => method.HasAttribute<System.Web.Http.HttpGetAttribute>() && method.IsPublic)
                                                                                .Where(method => method.ReturnType != typeof(void))
                                                                                .Select(method => new ClientApiUrl{Action = method.Name, Controller = x.Name.Replace("Controller", ""), IsWebApi = true})
                                                                 });

            urls.AddRange(apiUrls);
            return urls;
        }

        public class ClientApi
        {
            public string Name { get; internal set; }
            public IEnumerable<ClientApiUrl> Methods { get; internal set; }
        }

        public class ClientApiUrl
        {
            public string Action { get; internal set; }
            public string Controller { get; internal set; }
            public bool IsWebApi { get; set; }
        }
    }
}