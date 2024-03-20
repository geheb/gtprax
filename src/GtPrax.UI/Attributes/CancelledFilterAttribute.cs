namespace GtPrax.UI.Attributes;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class CancelledFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is OperationCanceledException)
        {
            context.ExceptionHandled = true;
            context.Result = new BadRequestResult();
        }
    }
}
