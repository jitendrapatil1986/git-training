namespace Warranty.UI.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Api;
    using Warranty.Core.Extensions;

    public class ClientApiHelper
    {
        public IEnumerable<ClientApi> GetUrls()
        {
            var urls= from type in typeof(ApiController).Assembly.GetTypes()
                   where (type.IsAssignableTo<Controller>() || type.IsAssignableTo<ApiController>()) && !type.IsAbstract
                   let controller = type.Name.Replace("Controller", "")
                   let methods = from method in type.GetMethods()
                                 where method.ReturnType.IsAssignableTo<ActionResult>()
                                 where !method.HasAttribute<HttpPostAttribute>()
                                 select new ClientApiUrl { Action = method.Name, Controller = controller }
                   select new ClientApi { Name = controller, Methods = methods };
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
        }
    }
}