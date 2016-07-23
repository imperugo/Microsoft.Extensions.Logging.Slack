using System;
using System.Net;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Logging.Slack.Tests
{
	public class SlackLoggerTest : IDisposable
	{
		private Mock<FakeHttpMessageHandler> msgHandler;
		private readonly string webhookUrl = "http://www.tostring.it/";
		private const string state = "This is a test, and {curly braces} are just fine!";
		private readonly Func<object, Exception, string> defaultFormatter = (state, exception) => state.ToString();
		private HttpClient client;

		private SlackLogger SetUp(Func<LogLevel, bool> filter)
		{
			msgHandler = new Mock<FakeHttpMessageHandler> {CallBase = true};
			msgHandler
				.Setup(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)))
				.Returns(new HttpResponseMessage(HttpStatusCode.NotFound));

			client = new HttpClient(msgHandler.Object);

			return new SlackLogger("testlog", filter, client, "testEnvironment", "testLogApplication", new Uri(webhookUrl));
		}

		public void Dispose()
		{
			this.client?.Dispose();
			this.client = null;
			this.msgHandler = null;
		}

		[Fact]
		public void LogsWhenNullFilterGiven()
		{
			// Arrange
			var logger = (ILogger) SetUp(null);
			msgHandler
				.Setup(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)))
				.Returns(new HttpResponseMessage(HttpStatusCode.NotFound));

			// Act
			logger.Log(LogLevel.Information, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void ThrowsException_WhenNoFormatterIsProvided()
		{
			// Arrange
			var logger = (ILogger) SetUp(null);

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => logger.Log<object>(LogLevel.Trace, 1, "empty", new Exception(), null));
		}

		[Fact]
		public void ThrowsException_WhenNoMessageAndExceptionAreProvided()
		{
			var logger = (ILogger) SetUp(null);

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => logger.LogCritical(0, null, message: null));
			Assert.Throws<ArgumentNullException>(() => logger.LogCritical(0, null));
		}

		[Fact]
		public void ThrowsException_WhenNoMessageIsProvided()
		{
			// Arrange
			var logger = (ILogger) SetUp(null);
			var exception = new InvalidOperationException("Invalid value");

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => logger.LogCritical(10, message: null, exception: exception));
		}

		[Fact]
		public void CriticalFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger) SetUp((logLevel) => logLevel >= LogLevel.Critical);

			// Act
			logger.Log(LogLevel.Warning, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Never);

			// Act
			logger.Log(LogLevel.Critical, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void ErrorFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger) SetUp((logLevel) => logLevel >= LogLevel.Error);

			// Act
			logger.Log(LogLevel.Warning, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Never);

			// Act
			logger.Log(LogLevel.Error, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void WarningFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger) SetUp((logLevel) => logLevel >= LogLevel.Warning);

			// Act
			logger.Log(LogLevel.Information, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Never);

			// Act
			logger.Log(LogLevel.Warning, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void InformationFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger) SetUp((logLevel) => logLevel >= LogLevel.Information);

			// Act
			logger.Log(LogLevel.Debug, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Never);

			// Act
			logger.Log(LogLevel.Information, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void DebugFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger)SetUp((logLevel) => logLevel >= LogLevel.Debug);

			// Act
			logger.Log(LogLevel.Trace, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Never);

			// Act
			logger.Log(LogLevel.Debug, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Once());
		}

		[Fact]
		public void TraceFilter_LogsWhenAppropriate()
		{
			// Arrange
			var logger = (ILogger)SetUp((logLevel) => logLevel >= LogLevel.Trace);

			// Act
			logger.Log(LogLevel.Critical, 0, state, null, defaultFormatter);
			logger.Log(LogLevel.Error, 0, state, null, defaultFormatter);
			logger.Log(LogLevel.Warning, 0, state, null, defaultFormatter);
			logger.Log(LogLevel.Information, 0, state, null, defaultFormatter);
			logger.Log(LogLevel.Debug, 0, state, null, defaultFormatter);
			logger.Log(LogLevel.Trace, 0, state, null, defaultFormatter);

			// Assert
			msgHandler
				.Verify(t => t.Send(It.Is<HttpRequestMessage>(
					msg =>
						msg.Method == HttpMethod.Post &&
						msg.RequestUri.ToString() == webhookUrl)), Times.Exactly(6));
		}
	}
}