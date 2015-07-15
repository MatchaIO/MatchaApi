using System.Net.Http.Headers;
using System.Web.Http;
using FluentValidation.WebApi;
using Matcha.WebApi.Filters;
using Newtonsoft.Json.Serialization;

namespace Matcha.WebApi.Config
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register()
        {
            var config = new HttpConfiguration();

            //validation 
            FluentValidationModelValidatorProvider.Configure(config);

            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));//for html request allow the return of json (ie web api calls from a browser can receive json back)
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            //Filters
            config.Filters.Add(new ValidateModelAttribute());
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            return config;
        }
    }
}
