namespace Homework.Models.Sports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Model for a generic sports team.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Team
    {
        /// <summary>
        /// Gets or sets the Unique id of the team
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of short names for the team.
        /// </summary>
        [JsonProperty("shortNames")]
        public List<string> ShortNames { get; set; }

        /// <summary>
        /// Gets or sets the conference that the team is in
        /// </summary>
        [JsonProperty("conference")]
        public string Conference { get; set; }

        /// <summary>
        /// Gets or sets the sport that the team participates in
        /// </summary>
        [JsonProperty("sport")]
        public string Sport { get; set; }

        /// <summary>
        /// Gets or sets the Name of the current team mascot 
        /// </summary>
        [JsonProperty("mascot")]
        public string Mascot { get; set; }
    }
}
