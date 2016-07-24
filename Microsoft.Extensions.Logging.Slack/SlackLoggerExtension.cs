using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Extensions.Logging.Slack
{
	public static class SlackLoggerExtension
	{
		public static ILoggerFactory AddSlack(this ILoggerFactory factory, SlackConfiguration configuration, string applicationName, string environmentName, HttpClient client = null)
		{
			if (string.IsNullOrEmpty(applicationName))
			{
				throw new ArgumentNullException(nameof(applicationName));
			}

			if (string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException(nameof(environmentName));
			}

			ILoggerProvider provider = new SlackLoggerProvider((n,l) => l >= configuration.MinLevel, configuration, client, applicationName, environmentName);

			factory.AddProvider(provider);

			return factory;
		}

		public static ILoggerFactory AddSlack(this ILoggerFactory factory, Func<string, LogLevel, bool> filter, SlackConfiguration configuration, string applicationName, string environmentName, HttpClient client = null)
		{
			if (string.IsNullOrEmpty(applicationName))
			{
				throw new ArgumentNullException(nameof(applicationName));
			}

			if (string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException(nameof(environmentName));
			}

			ILoggerProvider provider = new SlackLoggerProvider(filter,configuration, client, applicationName, environmentName);

			factory.AddProvider(provider);

			return factory;
		}

		public static ILoggerFactory AddSlack(this ILoggerFactory factory,  SlackConfiguration configuration, IHostingEnvironment hostingEnvironment, HttpClient client = null)
		{
			ILoggerProvider provider = new SlackLoggerProvider((n, l) => l >= configuration.MinLevel, configuration, client, hostingEnvironment.ApplicationName, hostingEnvironment.EnvironmentName);

			factory.AddProvider(provider);

			return factory;
		}

		public static ILoggerFactory AddSlack(this ILoggerFactory factory, Func<string, LogLevel, bool> filter, SlackConfiguration configuration, IHostingEnvironment hostingEnvironment, HttpClient client = null)
		{
			ILoggerProvider provider = new SlackLoggerProvider(filter, configuration, client, hostingEnvironment.ApplicationName, hostingEnvironment.EnvironmentName);

			factory.AddProvider(provider);

			return factory;
		}
	}
}