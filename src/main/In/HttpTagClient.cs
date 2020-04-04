using NLog;
using org.neurul.Common.Http;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Data.Tag.Client.In
{
    public class HttpTagClient : ITagClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpTagClient.logger.Error(ex, "Error occurred while communicating with ei8 Tag. " + ex.InnerException?.Message)
            );

        private static readonly string tagsPath = "data/tags/";
        private static readonly string tagsPathTemplate = tagsPath + "{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpTagClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task ChangeTag(string avatarUrl, string id, string newTag, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken)) =>
            await HttpTagClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.ChangeTagInternal(avatarUrl, id, newTag, expectedVersion, authorId, token).ConfigureAwait(false));

        private async Task ChangeTagInternal(string avatarUrl, string id, string newTag, int expectedVersion, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Tag = newTag,
                AuthorId = authorId
            };

            await this.requestProvider.PutAsync(
               $"{avatarUrl}{string.Format(HttpTagClient.tagsPathTemplate, id)}",
               data,
               token: token,
               headers: new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
