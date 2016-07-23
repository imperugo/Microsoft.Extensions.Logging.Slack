using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Logging.Slack
{
	public class SlackLogger : ILogger
	{
		private readonly Uri webhookUri;
		private readonly HttpClient httpClient;
		private readonly string applicationName;
		private readonly string environmentName;
		private readonly string name;
		private Func<LogLevel, bool> filter;

		public SlackLogger(string name, Func<LogLevel, bool> filter, 
									HttpClient httpClient, 
									string environmentName, 
									string applicationName, 
									Uri webhookUri)
		{
			Filter = filter ?? (logLevel => true);
			this.environmentName = environmentName;
			this.applicationName = applicationName;
			this.webhookUri = webhookUri;
			this.name = name;
			this.httpClient = httpClient;
		}

		private Func<LogLevel, bool> Filter
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

		/// <summary>
		/// Writes a log entry.
		/// </summary>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="eventId">Id of the event.</param>
		/// <param name="state">The entry to be written. Can be also an object.</param>
		/// <param name="exception">The exception related to this entry.</param>
		/// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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

			httpClient.PostAsync(this.webhookUri,new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Wait();
		}

		/// <summary>
		/// Checks if the given <paramref name="logLevel"/> is enabled.
		/// </summary>
		/// <param name="logLevel">level to be checked.</param>
		/// <returns><c>true</c> if enabled.</returns>
		public bool IsEnabled(LogLevel logLevel)
		{
			return Filter(logLevel);
		}

		/// <summary>
		/// Begins a logical operation scope.
		/// </summary>
		/// <param name="state">The identifier for the scope.</param>
		/// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}
}