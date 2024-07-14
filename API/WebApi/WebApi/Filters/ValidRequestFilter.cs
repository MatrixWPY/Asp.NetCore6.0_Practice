using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Models.Response;

namespace WebApi.Filters
{
    /// <summary>
    /// API傳入參數驗證
    /// </summary>
    public class ValidRequestAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                context.Result = new JsonResult
                (
                    new ApiResultRP<IEnumerable<string>>
                    {
                        Code = -1,
                        Msg = "Valid Error",
                        Result = context.ModelState.SelectMany(e => e.Value.Errors.Select(f => f.ErrorMessage))
                    }
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
