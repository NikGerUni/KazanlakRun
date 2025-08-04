using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace KazanlakRun.Web.Filters
{
    public class VolunteerExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<VolunteerExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public VolunteerExceptionFilter(
            ILogger<VolunteerExceptionFilter> logger,
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
                "Unhandled exception in VolunteerController.{Action}",
                actionName);

            context.ExceptionHandled = true;

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = actionName switch
            {
                nameof(VolunteerController.Index) =>
                    "Unable to load volunteers list. Please try again later.",
                nameof(VolunteerController.Create) =>
                    "Unable to create volunteer. Please try again later.",
                nameof(VolunteerController.Edit) =>
                    "Unable to edit volunteer. Please try again later.",
                nameof(VolunteerController.Delete) or "DeleteConfirmed" =>
                    "Unable to delete volunteer. Please try again later.",
                _ => "An error occurred while managing volunteers."
            };

            object? emptyModel = actionName switch
            {
                nameof(VolunteerController.Index) =>
                    new List<VolunteerViewModel>(),

                nameof(VolunteerController.Create) or
                nameof(VolunteerController.Edit) or
                nameof(VolunteerController.Delete) or "DeleteConfirmed" =>
                    new VolunteerViewModel
                    {
                        AllRoles = new List<SelectListItem>()
                    },

                _ => null
            };

            context.Result = new ViewResult
            {
                ViewName = actionName == "DeleteConfirmed" ? "Delete" : actionName,
                ViewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    context.ModelState)
                {
                    Model = emptyModel
                },
                TempData = tempData
            };
        }
    }
}

