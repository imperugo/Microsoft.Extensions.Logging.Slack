using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.Slack.Tests
{
	public class FakeHttpMessageHandler : HttpMessageHandler
	{
		public virtual HttpResponseMessage Send(HttpRequestMessage request)
		{
			throw new NotImplementedException("Rember to setup this method with your mocking framework");
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(Send(request));
		}
	}
}