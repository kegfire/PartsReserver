using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PartsReserver
{
	public class LoggingHandler : DelegatingHandler
	{
		public LoggingHandler(HttpMessageHandler innerHandler)
			: base(innerHandler)
		{
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Logger.Debug("Request:");
			Logger.Debug(request.ToString());
			if (request.Content != null)
			{
				Logger.Debug(await request.Content.ReadAsStringAsync());
			}

			var response = await base.SendAsync(request, cancellationToken);

			Logger.Debug("Response:");
			Logger.Debug(response.ToString());
			if (response.Content != null)
			{
				Logger.Debug(await response.Content.ReadAsStringAsync());
			}
			return response;
		}
	}
}
