using System;
using System.Net.Http;

namespace Microsoft.Extensions.Logging.Slack
{

	public class SlackLoggerProvider : ILoggerProvider
	{
		private readonly string applicationName;
		private readonly SlackConfiguration configuration;
		private readonly string environmentName;
		private readonly Func<LogLevel, bool> filter;
		private readonly HttpClient httpClient;

		public SlackLoggerProvider(Func<LogLevel, bool> filter, 
											SlackConfiguration configuration, 
											HttpClient httpClient,
			string applicationName, string environmentName)
		{
			this.filter = filter;
			this.configuration = configuration;
			this.httpClient = httpClient ?? new HttpClient();
			this.applicationName = applicationName;
			this.environmentName = environmentName;
		}

		public void Dispose()
		{
		}

		/// <summary>
		/// Creates a new <see cref="ILogger"/> instance.
		/// </summary>
		/// <param name="categoryName">The category name for messages produced by the logger.</param>
		/// <returns></returns>
		public ILogger CreateLogger(string categoryName)
		{
			return new SlackLogger(categoryName, filter, httpClient, environmentName, applicationName, configuration.WebhookUrl);
		}
	}
}