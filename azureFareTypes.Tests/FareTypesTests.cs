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
    public class FareTypesTests
    {
        protected ILogger Log => Mock.Of<ILogger>();

        protected FareTypesDbContext Db => new FareTypesDbContext(new DbContextOptionsBuilder<FareTypesDbContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options);

        protected HttpRequest HttpRequestSetup() =>
            HttpRequestSetup(string.Empty);

        protected HttpRequest HttpRequestSetup(string body) =>
            HttpRequestSetup(new Dictionary<string, StringValues>(), body, "get");

        protected HttpRequest HttpRequestSetup(Dictionary<string, StringValues> query, string body, string method)
        {
            var reqMock = new Mock<HttpRequest>();

            reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            reqMock.Setup(req => req.Body).Returns(stream);
            reqMock.Setup(req => req.Method).Returns(method);
            return reqMock.Object;
        }
    }
}
