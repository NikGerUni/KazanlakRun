using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KazanlakRun.Web.Filters
{
    public class UserExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<UserExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public UserExceptionFilter(
            ILogger<UserExceptionFilter> logger,
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
                "Unhandled exception in UsersController.{Action}",
                actionName);

            context.ExceptionHandled = true;

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = actionName switch
            {
                nameof(UsersController.Index) =>
                    "Unable to load the list of users. Please try again later.",
                nameof(UsersController.Edit) =>
                    "Unable to load or update the user role. Please try again later.",
                _ => "An error occurred while managing users."
            };

            object? emptyModel = actionName switch
            {
                nameof(UsersController.Index) =>
                    new List<UserRoleViewModel>(),

                nameof(UsersController.Edit) =>
                    new EditUserRoleViewModel
                    {
                        UserId = string.Empty,
                        Email = string.Empty,
                        Roles = new List<string>(),
                        SelectedRole = string.Empty
                    },

                _ => null
            };

            context.Result = new ViewResult
            {
                ViewName = actionName,
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
