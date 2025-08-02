using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KazanlakRun.Web.Filters
{
    public class ReportExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ReportExceptionFilter> _logger;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public ReportExceptionFilter(
            ILogger<ReportExceptionFilter> logger,
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
                "Unhandled exception in ReportController.{Action}",
                actionName);

            context.ExceptionHandled = true;

            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = actionName switch
            {
                nameof(ReportController.VolunteersByAidStation) =>
                    "Unable to load volunteers report. Please try again later.",
                nameof(ReportController.RunnersByAidStation) =>
                    "Unable to load runners report. Please try again later.",
                nameof(ReportController.GoodsByAidStation) =>
                    "Unable to load goods consumption report. Please try again later.",
                nameof(ReportController.GoodsForDelivery) =>
                    "Unable to load goods delivery report. Please try again later.",
                _ => "An error occurred while loading the report."
            };

            object? emptyModel = actionName switch
            {
                nameof(ReportController.VolunteersByAidStation) =>
                    new VolunteersByAidStationPageViewModel
                    {
                        Station = new AidStationVolunteersReportViewModel
                        {
                            AidStationName = "–",
                            Volunteers = new List<VolunteerReport>()
                        },
                        PageNumber = 1,
                        TotalPages = 1,
                        FilterText = string.Empty
                    },
                nameof(ReportController.RunnersByAidStation) =>
                    Enumerable.Empty<AidStationRunnersReportViewModel>(),
                nameof(ReportController.GoodsByAidStation) =>
                    Enumerable.Empty<AidStationGoodsReportViewModel>(),
                nameof(ReportController.GoodsForDelivery) =>
                    Enumerable.Empty<GoodsForDeliveryReportViewModel>(),
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
