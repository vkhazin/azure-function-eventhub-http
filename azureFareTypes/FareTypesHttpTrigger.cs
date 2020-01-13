using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azureFareTypes
{
    public class FareTypesHttpTrigger
    {
        private readonly FareTypesDbContext _db;
        private readonly IFareTypesCosmosDb _cosmosDb;

        public FareTypesHttpTrigger(FareTypesDbContext db, IFareTypesCosmosDb cosmosDb)
        {
            _db = db;
            _cosmosDb = cosmosDb;
        }

        [FunctionName("FareTypesHttpTriggerGet")]
        public async Task<IActionResult> RunGet(
            [HttpTrigger("get", Route = "faretypes/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation($"Get '{id}'");
            var fareType = await _cosmosDb.Get(id);

            return fareType != null
                ? (ActionResult)new OkObjectResult(fareType)
                : new NotFoundResult();
        }

        [FunctionName("FareTypesHttpTriggerGetAll")]
        public async Task<IActionResult> RunGetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "faretypes")] HttpRequest req,
            ILogger log)
        {
            int.TryParse(req.Query["skip"], out int skip);
            int.TryParse(req.Query["limit"], out int limit);
            limit = limit == 0 ? 20 : limit;

            log.LogInformation($"Get all, skip '{skip}', limit '{limit}'");
            return new OkObjectResult(await _cosmosDb.GetAll(skip, limit));
        }

        [FunctionName("FareTypesHttpTriggerDelete")]
        public async Task<IActionResult> RunDelete(
            [HttpTrigger("delete", Route = "faretypes/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation($"Delete '{id}'");
            return new StatusCodeResult(await _db.Delete(id)
                ? StatusCodes.Status200OK
                : StatusCodes.Status404NotFound);
        }

        [FunctionName("FareTypesHttpTriggerPost")]
        public async Task<IActionResult> RunPost(
            [HttpTrigger("post", Route = "faretypes")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FareType>(requestBody);

            if (data == null)
            {
                log.LogError($"Unknown Post body: '{requestBody}'");
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            log.LogInformation($"Post '{data.FareTypeId}'");
            return new StatusCodeResult(await _db.Insert(data)
                ? StatusCodes.Status201Created
                : StatusCodes.Status409Conflict);
        }

        [FunctionName("FareTypesHttpTriggerPut")]
        public async Task<IActionResult> RunPut(
            [HttpTrigger("put", Route = "faretypes/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FareType>(requestBody);

            if (data == null || data.FareTypeId != id)
            {
                log.LogError($"Unknown Put body: '{requestBody}'");
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            log.LogInformation($"Put '{id}'"); 
            return new StatusCodeResult(await _db.Upsert(id, data)
                ? StatusCodes.Status200OK
                : StatusCodes.Status404NotFound);
        }

        [FunctionName("FareTypesHttpTriggerPatch")]
        public async Task<IActionResult> RunPatch(
            [HttpTrigger("patch", Route = "faretypes/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FareType>(requestBody);

            if (data == null)
            {
                log.LogError($"Unknown Patch body: '{requestBody}'");
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            log.LogInformation($"Patch '{data.FareTypeId}'"); 
            return new StatusCodeResult(await _db.Update(id, data)
                ? StatusCodes.Status200OK
                : StatusCodes.Status404NotFound);
        }
    }
}