using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Logging.Slack
{
	public class SlackLogger : ILogger
	{
		private readonly SlackConfiguration configuration;
		private readonly HttpClient httpClient;
		private readonly string applicationName;
		private readonly string environmentName;
		private readonly string name;
		private Func<LogLevel, bool> filter;

		public SlackLogger(string name, Func<LogLevel, bool> filter, 
									SlackConfiguration configuration,
									HttpClient httpClient, 
									string environmentName, 
									string applicationName)
		{
			Filter = filter ?? (logLevel => true);
			this.configuration = configuration;
			this.environmentName = environmentName;
			this.applicationName = applicationName;
			this.name = name;
			this.httpClient = httpClient;
		}

		public Func<LogLevel, bool> Filter
		{
			get { return filter; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				filter = value;
			}
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			var color = "good";

			switch (logLevel)
			{
				case LogLevel.None:
				case LogLevel.Trace:
				case LogLevel.Debug:
				case LogLevel.Information:
					color = "good";
					break;
				case LogLevel.Warning:
					color = "warning";
					break;
				case LogLevel.Error:
				case LogLevel.Critical:
					color = "danger";
					break;
			}

			var title = formatter(state, exception);

			var exceptinon = exception?.ToString();

			var obj = new
			{
				attachments = new[]
				{
					new
					{
						fallback = $"[{applicationName}] [{environmentName}] [{name}] [{title}].",
						color,
						title,
						text = exceptinon,
						fields = new[]
						{
							new
							{
								title = "Project",
								value = applicationName,
								@short = "true"
							},
							new
							{
								title = "Environment",
								value = environmentName,
								@short = "true"
							}
						}
					}
				}
			};

			httpClient.PostAsync(configuration.WebhookUrl,
				new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Wait();
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return Filter(logLevel);
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}
}