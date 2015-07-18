using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Matcha.WebApi.Domain.DataAccess;

namespace Matcha.WebApi.Filters
{
    public class EntityNotFoundExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is EntityNotFoundException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, actionExecutedContext.Exception.Message);
            }
            base.OnException(actionExecutedContext);
        }
    }
}