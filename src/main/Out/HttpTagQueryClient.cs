using NLog;
using neurUL.Common.Http;
using Polly;
using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Data.Tag.Common;

namespace ei8.Data.Tag.Client.Out
{
    public class HttpTagQueryClient : ITagQueryClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpTagQueryClient.logger.Error(ex, "Error occurred while communicating with ei8 Tag. " + ex.InnerException?.Message)
            );
        private static readonly string GetTagsPathTemplate = "data/tags";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpTagQueryClient()
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task<ItemData> GetItemById(string avatarUrl, string id, CancellationToken token = default(CancellationToken)) =>
           await HttpTagQueryClient.exponentialRetryPolicy.ExecuteAsync(
               async () => await this.GetItemByIdInternal(avatarUrl, id, token).ConfigureAwait(false));
        
        private async Task<ItemData> GetItemByIdInternal(string avatarUrl, string id, CancellationToken token = default)
        {
            return await requestProvider.GetAsync<ItemData>(
                           $"{avatarUrl}{HttpTagQueryClient.GetTagsPathTemplate}/{id}",
                           token: token
                           );
        }
    }
}
