using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homework.FrontEnd.Models.Sports;
using Homework.Models.Sports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;

namespace Homework.FrontEnd.Controllers.api
{
    [Produces("application/json")]
    public class SportsController : Controller
    {
        private static readonly object InitializationLockObject = new object();

        private static DocumentClient client;

        private static bool isInitialized = false;

        private static string databaseName;

        private static string collectionName;

        private static Uri collectionUri;

        private readonly IConfiguration config;

        public SportsController(IConfiguration config)
        {
            this.config = config;

            Init(config);
        }

        private static void Init(IConfiguration config)
        {
            if (isInitialized)
            {
                return;
            }

            lock (InitializationLockObject)
            {
                if (isInitialized)
                {
                    return;
                }

                string endpoint = config["sports:db:endpoint"];
                string key = config["sports:db:key"];
                databaseName = config["sports:db:name"];
                collectionName = config["sports:db:collection"];
                collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);


                Dictionary<string, string> logProperties = new Dictionary<string, string>()
                {
                    [nameof(endpoint)] = endpoint,
                    ["keyPresent"] = (!string.IsNullOrWhiteSpace(key)).ToString(), // not storing the key,
                    [nameof(databaseName)] = databaseName,
                    [nameof(collectionName)] = collectionName
                };

                /*
                 * In a real application this 'key' variable would be 
                 * populated by a secret stored in Keyvault and be equipped 
                 * with a runtime certificate to authenticate against either
                 * AzureAd for a Token or directly with keyvault. 
                 * 
                 * The key would then be transported securely to this client
                 * where it can be used without storing it or injecting it
                 * into the configuration.
                 * 
                 * However, this is a demo application
                 * Thanks for reading :).
                 */

                try
                {
                    // TODO: Retry Logic (Use EnterpriseLibrary)
                    client = new DocumentClient(new Uri(endpoint), key);

                    Telemetry.Info("Initialized Sports Db Connection to CosmosDb.", "e792b662-0c3e-43ab-bb42-692b76a914b9", logProperties);

                    isInitialized = true;
                }
                catch (Exception ex)
                {
                    Telemetry.Error("An unexpected exception occured when attempting to initialize the DOcument client. ", "1b84bf04-5518-4bcd-978a-a7c63615a2e5", ex);
                }
            }
        }

        [HttpGet]
        [Route("api/homework/sports/{sport}/{team}/mascot")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetMascot(string sport, string team)
        {
            Team t = await GetTeamInternal(sport, team);

            if (t == null)
            {
                return this.NotFound();
            }

            return this.Ok(t.Mascot);
        }

        [HttpGet]
        [Route("api/homework/sports/{sport}/{team}")]
        public async Task<IActionResult> GetTeam(string sport, string team)
        {
            Team t = await GetTeamInternal(sport, team);

            if (t == null)
            {
                return this.NotFound();
            }

            return this.Ok(t);
        }

        [HttpPut]
        [Route("api/homework/sports/team")]
        public async Task<IActionResult> CreateTeam([FromBody]Team team)
        {
            // discard id 
            team.Id = Guid.NewGuid().ToString(); // Could do some sort of deterministic Id System here for team names

            Team t = await GetTeamInternal(team.Sport, team.Name);

            if (t != null)
            {
                return this.StatusCode(409, "The team provided already exists by name or shortname.");
            }

            if (team.ShortNames != null)
            {
                // force upper on creation for indexing
                for (int i = 0; i < team.ShortNames.Count; i++)
                {
                    team.ShortNames[i] = team.ShortNames[i].ToUpper();
                }
            }
            else
            {
                // safety net
                team.ShortNames = new List<string>() { team.Name.ToUpper() };
            }

            ResourceResponse<Document> result = await client.CreateDocumentAsync(collectionUri, team);

            return this.Ok(result.Resource.Id);
        }

        [HttpPost]
        [Route("api/homework/sports/team")]
        public async Task<IActionResult> UpdateTeam([FromBody]Team team)
        {

            Team t = await GetTeamInternal(team.Sport, team.Name);

            if (t == null)
            {
                return this.NotFound();
            }

            ResourceResponse<Document> result = await client.ReplaceDocumentAsync(collectionUri, team);

            return this.Ok(result.Resource.Id);
        }

        [HttpDelete]
        [Route("api/homework/sports/team/{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            Uri docUri = UriFactory.CreateDocumentUri(databaseName, collectionName, id);

            Team t = await client.ReadDocumentAsync<Team>(docUri);

            if (t == null)
            {
                return this.NotFound();
            }

            // can throw exceptions.
            await client.DeleteDocumentAsync(docUri);

            return this.Ok();
        }

        private static async Task<Team> GetTeamInternal(string sport, string team)
        {
            string uteam = team.ToUpper();
            string usport = sport.ToUpper();
            FeedResponse<Team> result = await client.CreateDocumentQuery<Team>(collectionUri)
                .Where(t => t.Sport.ToUpper() == usport && (t.Name.ToUpper() == uteam || (t.ShortNames != null && t.ShortNames.Contains(uteam))))
                .AsDocumentQuery()
                .ExecuteNextAsync<Team>();

            return result.FirstOrDefault();
        }
    }
}