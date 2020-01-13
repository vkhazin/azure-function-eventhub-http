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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes.Tests
{
    [TestClass()]
    public class FareTypesHttpTriggerTests : FareTypesTests
    {

        [TestMethod()]
        public async Task RunGet()
        {
            var cosmosDb = new FakeCosmosDb();

            await cosmosDb.Upsert(new FareType() { FareTypeId = 123 });

            var resultOk = await new FareTypesHttpTrigger(Db, cosmosDb).RunGet(req: HttpRequestSetup(), 123, log: Log);
            Assert.IsInstanceOfType(resultOk, typeof(OkObjectResult));

            var result404 = await new FareTypesHttpTrigger(Db, cosmosDb).RunGet(req: HttpRequestSetup(), 456, log: Log);
            Assert.IsInstanceOfType(result404, typeof(NotFoundResult));
        }

        [TestMethod()]
        public async Task RunGetAll()
        {
            var cosmosDb = new FakeCosmosDb();

            await cosmosDb.Upsert(new FareType() { FareTypeId = 123 });
            await cosmosDb.Upsert(new FareType() { FareTypeId = 456 });

            var resultOk = await new FareTypesHttpTrigger(Db, cosmosDb).RunGetAll(req: HttpRequestSetup(), log: Log);
            Assert.IsInstanceOfType(resultOk, typeof(OkObjectResult));

            var resultBody = ((OkObjectResult)resultOk).Value;
            Assert.IsInstanceOfType(resultBody, typeof(FareType[]));

            var results = (FareType[])resultBody;
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(1, results.Count(f => f.FareTypeId == 123));
        }

        [TestMethod()]
        public async Task RunPost()
        {
            var db = Db;
            var cosmosDb = new FakeCosmosDb();
            var body = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });

            var resultPost400 = await new FareTypesHttpTrigger(db, cosmosDb).RunPost(req: HttpRequestSetup(), log: Log);
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((StatusCodeResult)resultPost400).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db, cosmosDb).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));

            var resultPost409 = await new FareTypesHttpTrigger(db, cosmosDb).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status409Conflict, ((StatusCodeResult)resultPost409).StatusCode);
        }

        [TestMethod()]
        public async Task RunDelete()
        {
            var db = Db;
            var cosmosDb = new FakeCosmosDb();
            var body = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });

            var resultPost404 = await new FareTypesHttpTrigger(db, cosmosDb).RunDelete(req: HttpRequestSetup(), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status404NotFound, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db, cosmosDb).RunPost(req: HttpRequestSetup(body), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));

            var resultPost200 = await new FareTypesHttpTrigger(db, cosmosDb).RunDelete(req: HttpRequestSetup(body), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(0, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
        }

        [TestMethod()]
        public async Task RunPut()
        {
            var db = Db;
            var cosmosDb = new FakeCosmosDb();
            var body1 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });
            var body2 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123, Description = "zzz" });

            var resultPost404 = await new FareTypesHttpTrigger(db, cosmosDb).RunPut(req: HttpRequestSetup(body1), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost200 = await new FareTypesHttpTrigger(db, cosmosDb).RunPut(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));
        }

        [TestMethod()]
        public async Task RunPatch()
        {
            var db = Db;
            var cosmosDb = new FakeCosmosDb();
            var body1 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123 });
            var body2 = JsonConvert.SerializeObject(new FareType { FareTypeId = 123, Description = "zzz" });

            var resultPost404 = await new FareTypesHttpTrigger(db, cosmosDb).RunPatch(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status404NotFound, ((StatusCodeResult)resultPost404).StatusCode);

            var resultPost201 = await new FareTypesHttpTrigger(db, cosmosDb).RunPost(req: HttpRequestSetup(body1), log: Log);
            Assert.AreEqual(StatusCodes.Status201Created, ((StatusCodeResult)resultPost201).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(0, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));

            var resultPost200 = await new FareTypesHttpTrigger(db, cosmosDb).RunPatch(req: HttpRequestSetup(body2), 123, log: Log);
            Assert.AreEqual(StatusCodes.Status200OK, ((StatusCodeResult)resultPost200).StatusCode);
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.FareTypeId == 123));
            Assert.AreEqual(1, await db.FareTypes.CountAsync(ft => ft.Description == "zzz"));
        }
    }
}