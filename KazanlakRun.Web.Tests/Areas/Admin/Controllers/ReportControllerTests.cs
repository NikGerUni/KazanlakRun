﻿using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class ReportControllerTests
    {
        private Mock<IReportService> _serviceMock;
        private ReportController _controller;
        private ITempDataDictionary _tempData;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IReportService>();
            _controller = new ReportController(_serviceMock.Object, new NullLogger<ReportController>());

            var httpContext = new DefaultHttpContext();
            _tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = _tempData;
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable d) d.Dispose();
        }

        [Test]
        public async Task RunnersByAidStation_ReturnsViewWithModel()
        {
            var sample = new List<AidStationRunnersReportViewModel>
            {
                new AidStationRunnersReportViewModel
                {
                    AidStationName = "Station A",
                    Distances = new List<DistanceRunner>
                    {
                        new DistanceRunner { DistanceName = "5K", RegRunners = 10 }
                    }
                }
            };
            _serviceMock
                .Setup(s => s.GetRunnersByAidStationAsync())
                .ReturnsAsync(sample);

            var result = await _controller.RunnersByAidStation();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.AreSame(sample, view.Model);
        }

        [Test]
        public async Task GoodsByAidStation_ReturnsViewWithModel()
        {
            var sample = new List<AidStationGoodsReportViewModel>
            {
                new AidStationGoodsReportViewModel
                {
                    AidStationName = "Station B",
                    TotalRegisteredRunners = 5,
                    Goods = new List<GoodReport>
                    {
                        new GoodReport { Name = "Water", Measure = "pcs", QuantityPerAidStation = 5 }
                    }
                }
            };
            _serviceMock
                .Setup(s => s.GetGoodsByAidStationAsync())
                .ReturnsAsync(sample);

            var result = await _controller.GoodsByAidStation();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.AreSame(sample, view.Model);
        }

    }

}
