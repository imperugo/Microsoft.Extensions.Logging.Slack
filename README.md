# Microsoft.Extensions.Logging.Slack

This project is not a part of NET Core or ASP.NET Core and is not provider by Microsoft.
This is a logged implementation for `Microsoft.Extensions.Logging` who will log into [Slack](https://slack.com/).

The only reason why the name is starting with `Microsoft.Extensions.Logging` is for "convention" with the other loggers released by Microsoft.

Here http://tostring.it/2015/11/23/Log-your-application-errors-into-Slack/ more information about the approach.

**Right now is working with net451 & netstandard 1.3** [![NuGet Status](http://img.shields.io/nuget/v/Microsoft.Extensions.Logging.Slack.svg?style=flat)](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Slack/)

```
PM> Install-Package Microsoft.Extensions.Logging.Slack/
```

The package is almost done, but not release yet (need to write the unit tests), soon on nuget.

How to use it?

```cs
public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
{
  var configuration = new SlackConfiguration()
  {
    WebhookUrl = "http://tostring.it", // Read here how to retrieve it here http://tostring.it/2015/11/23/Log-your-application-errors-into-Slack/
    MinLevel = MinLevel.Error
  }
  app.AddSlack(configuration, env)
}
```

or

```cs
public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
{
  var configuration = new SlackConfiguration()
  {
    WebhookUrl = "http://tostring.it", // Read here how to retrieve it here http://tostring.it/2015/11/23/Log-your-application-errors-into-Slack/
    MinLevel = MinLevel.Error
  }
  app.AddSlack(configuration, env, "my application name", "my application environment")
}
```
Have fun!

