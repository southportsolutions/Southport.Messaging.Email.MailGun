# Southport.Messaging.Email.MailGun

[![NuGet](https://img.shields.io/nuget/v/Southport.Messaging.Email.MailGun.svg)](https://www.nuget.org/packages/Southport.Messaging.Email.MailGun/)

A [Mailgun](https://www.mailgun.com/) implementation of
[Southport.Messaging.Email.Core](https://www.nuget.org/packages/Southport.Messaging.Email.Core).
It sends email through the Mailgun Messages API using a fluent message builder, with
per-recipient Handlebars substitutions, Mailgun stored templates, attachments, and a
test-address redirect for non-production environments.

Targets .NET 9.0 and .NET 10.0. Licensed under MIT.

## Installation

Available on [nuget.org](https://www.nuget.org/packages/Southport.Messaging.Email.MailGun/).

```shell
dotnet add package Southport.Messaging.Email.MailGun
```

## Configuration

The library reads a `MailGunOptions` instance:

| Property             | Required | Description                                                                 |
|----------------------|----------|-----------------------------------------------------------------------------|
| `ApiKey`             | Yes      | Mailgun private API key. Sent as HTTP basic auth with user name `api`.       |
| `Domain`             | Yes      | Mailgun sending domain used when `Send()` is called without a domain.        |
| `TestEmailAddresses` | No       | Comma-separated list. When set, every message is redirected to these addresses instead of the real recipients. See [Test email addresses](#test-email-addresses). |

Example `appsettings.json` section:

```json
{
  "MailGun": {
    "ApiKey": "key-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "Domain": "mail.example.com",
    "TestEmailAddresses": ""
  }
}
```

## Registering with dependency injection

Both overloads register `IMailGunMessageFactory` as a typed `HttpClient` service.

```csharp
// From a configuration section
builder.Services.AddMailGunServices(builder.Configuration.GetSection("MailGun"));

// Or from an options instance
builder.Services.AddMailGunServices(new MailGunOptions
{
    ApiKey = "key-xxxxxxxx",
    Domain = "mail.example.com"
});
```

`IMailGunMessageFactory` extends the core `IEmailMessageFactory`, so code written against
the core abstractions can take either interface.

## Sending a message

```csharp
public class WelcomeMailer
{
    private readonly IMailGunMessageFactory _messageFactory;

    public WelcomeMailer(IMailGunMessageFactory messageFactory)
    {
        _messageFactory = messageFactory;
    }

    public async Task SendAsync(string toAddress, CancellationToken cancellationToken)
    {
        await using var message = _messageFactory.Create();

        var results = await message
            .SetFromAddress("no-reply@example.com", "Example")
            .AddToAddress(toAddress)
            .SetSubject("Welcome")
            .SetText("Thanks for signing up.")
            .SetHtml("<p>Thanks for signing up.</p>")
            .Send(cancellationToken: cancellationToken);

        foreach (var result in results)
        {
            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.Message);
            }
        }
    }
}
```

`Send()` makes one request to Mailgun per recipient and returns one `IEmailResult` per
recipient. Each result carries the recipient, a success flag, the raw Mailgun response in
`Message`, and the body that was sent in `MessageBody`.

`Send()` throws `SouthportMessagingException` when the from address is missing, when there
are no valid recipients, or when the message has no text, HTML, AMP HTML, or template.

### Send overloads

```csharp
Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default);
Task<IEnumerable<IEmailResult>> Send(bool substitute = true, CancellationToken cancellationToken = default);
Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default);
```

Pass `substitute: false` to skip Handlebars rendering and send the body as written. Pass a
`domain` to send from a domain other than the one in `MailGunOptions`.

## Substitutions

Subject, text, HTML, and AMP HTML bodies are rendered with
[Handlebars.Net](https://github.com/Handlebars-Net/Handlebars.Net) before sending.
Substitutions can be set per recipient or at message level. Recipient values override
message-level values with the same key.

```csharp
var recipients = new List<IEmailRecipient>
{
    new EmailRecipient("ann@example.com", substitutions: new Dictionary<string, object> { ["FirstName"] = "Ann" }),
    new EmailRecipient("ben@example.com", substitutions: new Dictionary<string, object> { ["FirstName"] = "Ben" })
};

await using var message = _messageFactory.Create();
var results = await message
    .SetFromAddress("no-reply@example.com")
    .AddToAddresses(recipients)
    .SetSubject("Hello {{FirstName}}")
    .SetText("Dear {{FirstName}}, this is a test email.")
    .AddSubstitution("Company", "Example Co")
    .Send();
```

## Mailgun stored templates

Reference a template stored in Mailgun with `SetTemplate`. Substitutions are passed to
Mailgun as template variables rather than rendered locally. `SetTemplateVersion` and
`SetTemplateText` map to the Mailgun `t:version` and `t:text` options.

```csharp
var recipient = new EmailRecipient("ann@example.com", substitutions: new Dictionary<string, object>
{
    ["name"] = "Ann Example",
    ["states"] = new List<string> { "CA", "CT", "TN" }
});

await using var message = _messageFactory.Create();
var results = await message
    .SetFromAddress("no-reply@example.com")
    .AddToAddress(recipient)
    .SetSubject("Your account")
    .SetTemplate("welcome_template")
    .Send();
```

## Attachments

Attachment types come from the core package. Attachments can be added at message level or
per recipient.

```csharp
await using var pdfStream = File.OpenRead("report.pdf");

await using var message = _messageFactory.Create();
var results = await message
    .SetFromAddress("no-reply@example.com")
    .AddToAddress("ann@example.com")
    .SetSubject("Monthly report")
    .SetText("The report is attached.")
    .AddAttachments(new EmailAttachmentString("Plain text content.", "notes.txt"))
    .AddAttachments(new EmailAttachmentStream(pdfStream, "report.pdf", "application/pdf"))
    .Send();
```

Stream and byte-array attachments are sent in full in every per-recipient request.

## Mailgun-specific options

| Method                              | Mailgun field          |
|-------------------------------------|------------------------|
| `SetAmpHtml(string)`                | `amp-html`             |
| `SetTag(string)` / `SetTags(list)`  | `o:tag`                |
| `SetDkim(bool)`                     | `o:dkim`               |
| `SetDeliveryTime(DateTime)`         | `o:deliverytime`       |
| `SetTestMode(bool)`                 | `o:testmode`           |
| `SetTracking(bool)`                 | `o:tracking`           |
| `SetTrackingClicks(bool)`           | `o:tracking-clicks`    |
| `SetTrackingOpens(bool)`            | `o:tracking-opens`     |
| `SetRequireTls(bool)`               | `o:require-tls`        |
| `SetSkipVerification(bool)`         | `o:skip-verification`  |
| `AddHeader(key, value)`             | `h:<key>`              |
| `AddCustomArgument(key, value)`     | `v:<key>`              |
| `SetReplyTo(string)`                | `h:Reply-To`           |

## Test email addresses

When `TestEmailAddresses` is set, every To, CC, and BCC recipient is replaced with the
listed addresses. Each original To recipient produces one message per test address, keeping
that recipient's substitutions, custom arguments, and attachments. The custom argument
`IsTest=true` is added to each redirected message. Use this in development and staging to
prevent mail reaching real users.

## Building and testing

```shell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

The tests send real messages through Mailgun. They need an API key and domain, read from
either a `MailGun` section in an `appsettings.json` file at the test project root (ignored by
git) or the `MAILGUNAPIKEY` and `MAILGUNDOMAIN` environment variables. Several tests expect a
stored template named `test_template` to exist on the domain.

## Releases

The GitHub Actions workflow builds, tests, and packs on every push and pull request to
`main`. Pushing a tag of the form `release@<version>` publishes that version to nuget.org.

## Contributing

Open an issue or pull request on
[GitHub](https://github.com/southportsolutions/Southport.Messaging.Email.MailGun).
