using AutoMapper;
using CDRApi.Controllers;
using CDRApi.Model;
using CDRApi.Services;
using CDRModel;
using CDRServices;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;

namespace CDRTest
{
    public class ControllerTests
    {
        private const string DefaultCallerId = "abcd";
        private const string DefaultReference = "12345";

        static CDRController GetController()
        {
            var logger = new LoggerFactory().CreateLogger<CDRController>();

            var repository = Substitute.For<ICDRRepository>();
            repository.Save(Arg.Any<IEnumerable<Call>>(), Arg.Any<CancellationToken>()).Returns(1);
            repository.Find(DefaultReference, Arg.Any<CancellationToken>()).Returns(new Call { CallerId = DefaultCallerId, Reference = DefaultReference, Type = CallType.Domestic });
            repository.CallerStats("abcd", Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CallType?>(), Arg.Any<CancellationToken>()).Returns(new List<CallerStats> { new CallerStats { Reference = DefaultReference, Type = CallType.Domestic } });
            repository.TopCallStats("abcd", Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CallType?>(), Arg.Any<CancellationToken>()).Returns(new List<CallerStats> { new CallerStats { Reference = DefaultReference, Type = CallType.Domestic } });

            var mapper = Substitute.For<IMapper>();
            mapper.Map<CallDto, Call>(Arg.Any<CallDto>()).Returns(new Call { CallerId = DefaultCallerId, Reference = DefaultReference, Type = CallType.Domestic });
            mapper.Map<Call, CallDto>(Arg.Any<Call>()).Returns(new CallDto { Caller_Id = DefaultCallerId, Reference = DefaultReference, Type = 1 });
            mapper.Map<CallerStats, CallerStatsDto>(Arg.Any<CallerStats>()).Returns(new CallerStatsDto { Reference = DefaultReference, Type = 1 });

            var mediator = Substitute.For<IMediator>();
            mediator.Publish(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var controller = new CDRController(logger, repository, mapper, mediator);

            return controller;
        }

        [Fact]
        public async Task CanPost()
        {
            var parser = Substitute.For<ICsvParser>();
            parser.IgnoreInvalidLines.Returns(true);
            parser.Parse(Arg.Any<IFormFile>(), Arg.Any<CancellationToken>()).Returns(new List<CallDto> { new CallDto { Caller_Id = DefaultCallerId, Reference = DefaultReference, Type = 1 } });

            var lines = "caller_id,recipient,call_date,end_time,duration,cost,reference,currency,type\r\n441216000000,448000000000,16/08/2016,14:21:33,43,0,C5DA9724701EEBBA95CA2CC5617BA93E4,GBP,2\r\n442036000000,44800833833,16/08/2016,14:00:47,244,0,C50B5A7BDB8D68B8512BB14A9D363CAA1,GBP,2";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(lines));

            var controller = GetController();

            var result = await controller.Post(new FormFile(stream, 0, stream.Length, "test", "test.csv"), parser, default);

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CanGet()
        {
            var controller = GetController();

            var result = await controller.Get(DefaultReference, default);

            Assert.NotNull(result);

            Assert.NotNull(result.Result);
            Assert.IsType<OkObjectResult>(result.Result);

            Assert.NotNull((result.Result as OkObjectResult)!.Value);
            Assert.IsType<CallDto>((result.Result as OkObjectResult)!.Value);

            Assert.Equal(DefaultReference, ((CallDto)(result.Result as OkObjectResult)!.Value!).Reference);
        }

        [Fact]
        public async Task NotFound()
        {
            var controller = GetController();

            var result = await controller.Get("abcde", default);

            Assert.NotNull(result);

            Assert.NotNull(result.Result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CanGetCallerStats()
        {
            var controller = GetController();

            var result = await controller.CallerStats(default, "abcd", DateTime.Today.AddDays(-1), DateTime.Today, null);

            Assert.NotNull(result);

            Assert.NotNull(result.Result);
            Assert.IsType<OkObjectResult>(result.Result);

            Assert.NotNull((result.Result as OkObjectResult)!.Value);
            Assert.IsType<List<CallerStatsDto>>((result.Result as OkObjectResult)!.Value);

            Assert.All(((result.Result as OkObjectResult)!.Value as List<CallerStatsDto>)!, x => Assert.Equal(DefaultReference, x.Reference));
        }

        [Fact]
        public async Task CanGetTopCallStats()
        {
            var controller = GetController();

            var result = await controller.TopCallStats(default, DefaultCallerId, 10, DateTime.Today.AddDays(-1), DateTime.Today, CallType.Domestic);

            Assert.NotNull(result);

            Assert.NotNull(result.Result);
            Assert.IsType<OkObjectResult>(result.Result);

            Assert.NotNull((result.Result as OkObjectResult)!.Value);
            Assert.IsType<List<CallerStatsDto>>((result.Result as OkObjectResult)!.Value);

            Assert.All(((result.Result as OkObjectResult)!.Value as List<CallerStatsDto>)!, x => Assert.Equal(DefaultReference, x.Reference));
        }
    }
}