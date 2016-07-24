# Microsoft.Extensions.Logging.Slack

If you are a Slack addicted this is the logger for you!

This project is not a part of NET Core or ASP.NET Core and is not provider by Microsoft.
This is a logged implementation for `Microsoft.Extensions.Logging` who will log into [Slack](https://slack.com/).

The only reason why the name is starting with `Microsoft.Extensions.Logging` is for "convention" with the other loggers released by Microsoft.

Here http://tostring.it/2015/11/23/Log-your-application-errors-into-Slack/ more information about the approach.

**Right now is working with net451 & netstandard 1.3** [![NuGet Status](http://img.shields.io/nuget/v/Microsoft.Extensions.Logging.Slack.svg?style=flat)](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Slack/)

The output should be something like this

![output](http://tostring.it/assets/2015/11/SlackLog/005.jpg)

```
PM> Install-Package Microsoft.Extensions.Logging.Slack
```

The package is almost done, but not release yet (need to write the unit tests), soon on nuget.

How to use it?

```cs
public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
{
  var configuration = new SlackConfiguration()
  {
    WebhookUrl = "http://tostring.it",
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
    WebhookUrl = "http://tostring.it",
    MinLevel = MinLevel.Error
  }
  app.AddSlack(configuration, env, "my application name", "my application environment")
}
```

To retrieve the right webhook url, follow the steps below:

![001](http://tostring.it/assets/2015/11/SlackLog/001.jpg)
![002](http://tostring.it/assets/2015/11/SlackLog/002.jpg)
![003](http://tostring.it/assets/2015/11/SlackLog/003.jpg)
![004](http://tostring.it/assets/2015/11/SlackLog/004.jpg)

Have fun!

