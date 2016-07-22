using System;

namespace Microsoft.Extensions.Logging.Slack
{
	public class SlackConfiguration
	{
		public Uri WebhookUrl { get; set; }
		public LogLevel MinLevel { get; set; }
		public string ApplicationName { get; set; }
	}
}