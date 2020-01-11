using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes.Tests
{
    [TestClass()]
    public class FareTypesHttpTriggerTests
    {
        private ILogger Log => Mock.Of<ILogger>();

        private FareTypesDbContext Db => new FareTypesDbContext(new DbContextOptionsBuilder<FareTypesDbContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options);

        [TestMethod()]
        public async Task RunGet_404()
        {
            var result = await new FareTypesHttpTrigger(Db).RunGet(req: HttpRequestSetup(), 123, log: Log);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public async Task RunPost()
        {
            var db = Db;
            var body = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });

            var resultPost400 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(), log: Log);
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((StatusCodeResult)resultPost400).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));

            var resultPost409 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status409Conflict, ((StatusCodeResult)resultPost409).StatusCode);
        }

        [TestMethod()]
        public async Task RunDelete()
        {
            var db = Db;
            var body = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });

            var resultPost404 = await new FareTypesHttpTrigger(db).RunDelete(req: HttpRequestSetup(), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status404NotFound, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));

            var resultPost200 = await new FareTypesHttpTrigger(db).RunDelete(req: HttpRequestSetup(body), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(0, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
        }

        [TestMethod()]
        public async Task RunPut()
        {
            var db = Db;
            var body1 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });
            var body2 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123, Description = "zzz" });

            var resultPost404 = await new FareTypesHttpTrigger(db).RunPut(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status404NotFound, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(body1), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(0, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));

            var resultPost200 = await new FareTypesHttpTrigger(db).RunPut(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));
        }

        [TestMethod()]
        public async Task RunPatch()
        {
            var db = Db;
            var body1 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });
            var body2 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123, Description = "zzz" });

            var resultPost404 = await new FareTypesHttpTrigger(db).RunPatch(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status404NotFound, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db).RunPost(req: HttpRequestSetup(body1), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(0, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));

            var resultPost200 = await new FareTypesHttpTrigger(db).RunPatch(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));
        }

        public HttpRequest HttpRequestSetup() =>
            HttpRequestSetup(string.Empty);

        public HttpRequest HttpRequestSetup(string body) =>
            HttpRequestSetup(new Dictionary<string, StringValues>(), body);

        public HttpRequest HttpRequestSetup(Dictionary<string, StringValues> query, string body)
        {
            var reqMock = new Mock<HttpRequest>();

            reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            return reqMock.Object;
        }
    }
}