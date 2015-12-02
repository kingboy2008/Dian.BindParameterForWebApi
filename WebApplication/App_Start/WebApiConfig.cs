using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.ParameterBindingRules.Insert(0, Dian.BindParameterForWebApi.Binding.ComplexMultiParameterBinding.CreateBindingForMarkedParameters);
            config.ParameterBindingRules.Insert(1, Dian.BindParameterForWebApi.Binding.ComplexSingleParameterBinding.CreateBindingForMarkedParameters);
            config.ParameterBindingRules.Insert(2, Dian.BindParameterForWebApi.Binding.MultiParameterBinding.CreateBindingForMarkedParameters);

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
