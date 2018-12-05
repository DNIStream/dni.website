using System.Collections.Generic;
using System.Reflection;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Email;
using DNI.Testing;

using Xunit;

namespace DNI.Services.Tests {
    /// <summary>
    ///     Unit tests for <see cref="EmailTemplateServiceBase" /> base class / shared methods.
    /// </summary>
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class EmailTemplateServiceBaseTests {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private IEmailService GetService() {
            return _fixture.Create<EmailTemplateServiceBase>();
        }

        [Fact]
        public void ReadTemplateResource_ReadsValidEmbeddedResourceFromCallingAssembly() {
            // Arrange
            var service = GetService();
            var assembly = Assembly.GetExecutingAssembly();
            const string resourcePath = "DNI.Services.Tests.TestResources.contact.json";
            var expectedContent = new EmailTemplate {
                subject = "This is a {{test1}}",
                preHeaderHtml = "This is a {{test2}}",
                introHtml = "This is a {{test3}}",
                outroHtml = "This is a {{test4}}",
                from = "from@test.com",
                actionLink = "http://www.test.com",
                actionText = "Test Link",
                plainTextBody = "This is a plain text body with {{test1}} and {{test4}} replacements"
            };

            // Act
            var resourceContent = service.ReadTemplateResource(assembly, resourcePath);

            // Assert
            Assert.NotNull(resourceContent);
            Assert.Equal(expectedContent.preHeaderHtml, resourceContent.preHeaderHtml);
            Assert.Equal(expectedContent.introHtml, resourceContent.introHtml);
            Assert.Equal(expectedContent.outroHtml, resourceContent.outroHtml);
            Assert.Equal(expectedContent.subject, resourceContent.subject);
            Assert.Equal(expectedContent.from, resourceContent.from);
            Assert.Equal(expectedContent.actionLink, resourceContent.actionLink);
            Assert.Equal(expectedContent.actionText, resourceContent.actionText);
            Assert.Equal(expectedContent.plainTextBody, resourceContent.plainTextBody);
        }

        [Fact]
        public void ReadLayoutResource_ReadsValidEmbeddedResourceFromCallingAssembly() {
            // Arrange
            var service = GetService();
            var assembly = Assembly.GetExecutingAssembly();
            const string resourcePath = "DNI.Services.Tests.TestResources.layout.html";
            const string expectedContent = @"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">{{preHeaderHtml}}</div>
<div class=""introHtml"">{{introHtml}}</div>
<div class=""actionLink"">{{actionLink}}</div>
<div class=""actionText"">{{actionText}}</div>
<div class=""outroHtml"">{{outroHtml}}</div>
</body>

</html>";

            // Act
            var resourceContent = service.ReadLayoutResource(assembly, resourcePath);

            // Assert
            Assert.NotNull(resourceContent);
            Assert.Equal(expectedContent, resourceContent);
        }

        [Fact]
        public void ReplacePlaceholders_ReplacesAllPlaceholdersWithSpecifiedText() {
            // Arrange
            var service = GetService();

            var actualContent = new EmailTemplate {
                subject = "Subject with a {{subject}}",
                introHtml = "Intro with a {{intro}}",
                outroHtml = "Outro with a {{outro}}",
                preHeaderHtml = "PreHeader with a {{preheader}}",
                actionLink = "Action with a {{actionLink}}",
                from = "From with a {{from}}",
                actionText = "Action text with a {{actionText}}",
                plainTextBody = "Plain text body with a {{testReplacement}}"
            };
            var replacements = new Dictionary<string, string> {
                {"subject", "monkey"},
                {"intro", "marmoset"},
                {"outro", "elephant"},
                {"preheader", "butterfly"},
                {"actionLink", "winkle"},
                {"from", "vole"},
                {"actionText", "parrot"},
                {"testReplacement", "guinea pig"}
            };

            // Act
            var resourceContent = service.ReplacePlaceholders(actualContent, replacements);

            // Assert
            Assert.NotNull(resourceContent);
            Assert.Equal("Subject with a monkey", actualContent.subject);
            Assert.Equal("Intro with a marmoset", actualContent.introHtml);
            Assert.Equal("Outro with a elephant", actualContent.outroHtml);
            Assert.Equal("PreHeader with a butterfly", actualContent.preHeaderHtml);
            Assert.Equal("Action with a winkle", actualContent.actionLink);
            Assert.Equal("From with a vole", actualContent.from);
            Assert.Equal("Action text with a parrot", actualContent.actionText);
            Assert.Equal("Plain text body with a guinea pig", actualContent.plainTextBody);
        }

        #region CompileHtmlBody

        [Fact]
        public void CompileHtmlBody_ReplacesLayoutPlaceholdersWithTemplateMembers() {
            // Arrange
            var service = GetService();
            const string layoutHtml = @"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">{{preHeaderHtml}}</div>
<div class=""introHtml"">{{introHtml}}</div>
<div class=""actionLink"">{{actionLink}}</div>
<div class=""actionText"">{{actionText}}</div>
<div class=""outroHtml"">{{outroHtml}}</div>
</body>

</html>";
            var template = new EmailTemplate {
                outroHtml = "test outro",
                introHtml = "test intro",
                preHeaderHtml = "test preheader",
                actionLink = "test action link",
                from = "test from",
                actionText = "test action text",
                subject = "test subject"
            };

            var expectedBody = $@"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">{template.preHeaderHtml}</div>
<div class=""introHtml"">{template.introHtml}</div>
<div class=""actionLink"">{template.actionLink}</div>
<div class=""actionText"">{template.actionText}</div>
<div class=""outroHtml"">{template.outroHtml}</div>
</body>

</html>";

            // Act
            var body = service.CompileHtmlBody(layoutHtml, template, null);

            // Arrange
            Assert.Equal(expectedBody, body);
        }

        [Fact]
        public void CompileHtmlBody_ReplacesLayoutPlaceholdersWithSpecifiedReplacements() {
            // Arrange
            var service = GetService();
            const string layoutHtml = @"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">{{preHeaderHtml}}</div>
<div class=""introHtml"">{{introHtml}}</div>
<div class=""actionLink"">{{actionLink}}</div>
<div class=""actionText"">{{actionText}}</div>
<div class=""outroHtml"">{{outroHtml}}</div>
{{layoutReplacement}}
</body>

</html>";
            var template = new EmailTemplate {
                outroHtml = "test outro",
                introHtml = "test intro",
                preHeaderHtml = "test preheader",
                actionLink = "test action link",
                from = "test from",
                actionText = "test action text",
                subject = "test subject"
            };

            var layoutReplacements = new Dictionary<string, string> {
                {"layoutReplacement", "Layout replacement test"}
            };
            var expectedBody = $@"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">{template.preHeaderHtml}</div>
<div class=""introHtml"">{template.introHtml}</div>
<div class=""actionLink"">{template.actionLink}</div>
<div class=""actionText"">{template.actionText}</div>
<div class=""outroHtml"">{template.outroHtml}</div>
Layout replacement test
</body>

</html>";

            // Act
            var body = service.CompileHtmlBody(layoutHtml, template, layoutReplacements);

            // Arrange
            Assert.Equal(expectedBody, body);
        }

        #endregion
    }
}