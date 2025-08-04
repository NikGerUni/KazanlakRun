using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KazanlakRun.Web.Filters
{
    public class RoleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<RoleExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public RoleExceptionFilter(
            ILogger<RoleExceptionFilter> logger,
            ITempDataDictionaryFactory tempDataFactory)
        {
            _logger = logger;
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
        {
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "";

            _logger.LogError(
                context.Exception,
                "Unhandled exception in RoleController.{Action}",
                actionName);

            context.ExceptionHandled = true;

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = actionName switch
            {
                nameof(RoleController.Index) =>
                    "Unable to load the roles list. Please try again later.",
                nameof(RoleController.SaveAll) =>
                    "Unable to save roles. Please try again later.",
                nameof(RoleController.RowTemplate) =>
                    "Unable to generate a new role row. Please try again later.",
                _ => "An error occurred while managing roles."
            };

            object? emptyModel = actionName switch
            {
                nameof(RoleController.Index) or nameof(RoleController.SaveAll) =>
                    new List<RoleViewModel>(),

                nameof(RoleController.RowTemplate) =>
                    new RoleViewModel(),

                _ => null
            };

            context.Result = actionName switch
            {
                nameof(RoleController.RowTemplate) => new PartialViewResult
                {
                    ViewName = "_RoleRow",
                    ViewData = new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        context.ModelState)
                    {
                        Model = emptyModel,
                        ["idx"] = "__index__"
                    },
                    TempData = tempData
                },

                _ => new ViewResult
                {
                    ViewName = actionName,
                    ViewData = new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        context.ModelState)
                    {
                        Model = emptyModel
                    },
                    TempData = tempData
                }
            };
        }
    }
}
