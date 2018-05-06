using chktr.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace chktr.Filters
{
    public sealed class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var model = new ModelError(context.ModelState);
                System.Console.WriteLine(string.Join('\r', model.Detail));
                context.Result = new BadRequestObjectResult(model);
            }
        }
    }
}