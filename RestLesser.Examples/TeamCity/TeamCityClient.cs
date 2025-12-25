using System;
using System.IO;
using System.Net;
using System.Net.Http;
using RestLesser.Authentication;
using RestLesser.DataAdapters;
using RestLesser.Examples.TeamCity.Models;

namespace RestLesser.Examples.TeamCity
{
    public class TeamCityClient : RestClient
    {
        private TeamCityClient(string url, IAuthentication authentication) : base(url, authentication,
            new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            },
            AdapterFactory.Get(Adapter.Xml))
        {
            Client.Timeout = TimeSpan.FromMinutes(5.0);
        }

        /// <summary>
        /// Token (Bearer) authentication
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        public TeamCityClient(string url, string token) : this(url, new TeamCityAuthentication(url, token))
        {
        }

        /// <summary>
        /// Basic authentication
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public TeamCityClient(string url, string username, string password) : this(url, new TeamCityAuthentication(url, username, password))
        {
        }

        /// <summary>
        /// Trigger a <see cref="Build"/>
        /// </summary>
        /// <param name="build"><see cref="Build"/> to trigger</param>
        /// <param name="moveToTop">Move the build to the top of the queue</param>
        /// <returns><see cref="Build"/>Updated build</returns>
        public Build TriggerBuild(Build build, bool moveToTop = true)
        {
            return Post<Build, Build>($"httpAuth/app/rest/buildQueue?moveToTop={moveToTop.ToString().ToLower()}", build);
        }

        /// <summary>
        /// Fetch a <see cref="Build"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Build FetchBuild(int id)
        {
            return Get<Build>($"httpAuth/app/rest/builds/id:{id}");       
        }

        /// <summary>
        /// Get latest buil id for a given build type
        /// </summary>
        /// <param name="buildType"></param>
        /// <returns></returns>
        public int GetLatestBuildId(string buildType)
        {
            return Get<int>($"httpAuth/app/rest/buildTypes/{buildType}/builds/status:success/id");
        }

        /// <summary>
        /// Fetch an artifact from the given build
        /// </summary>
        /// <param name="id"></param>
        /// <param name="artifact"></param>
        /// <param name="accept"></param>
        /// <param name="output"></param>
        public void FetchArtifact(int id, string artifact, string accept, Stream output)
        {
            GetFile($"httpAuth/app/rest/builds/id:{id}/artifacts/content/{artifact}", output, accept);
        }

        /// <summary>
        /// Get artifact from the latest build of <paramref name="buildType"/>
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="artifact"></param>
        /// <param name="accept"></param>
        /// <param name="output"></param>
        public void GetLatestArtifact(string buildType, string artifact, string accept, Stream output)
        {
            var buildId = GetLatestBuildId(buildType);
            FetchArtifact(buildId, artifact, accept, output);
        }

        /// <summary>
        /// Get artifact from the latest build of <paramref name="buildType"/>,
        /// streamed to memory.
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="artifact"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public MemoryStream GetLatestArtifact(string buildType, string artifact, string accept)
        {
            var output = new MemoryStream();
            GetLatestArtifact(buildType, artifact, accept, output);
            return output;
        }
    }
}
