﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.MailGun
{
    public interface IMailGunMessage : IEmailMessageCore
    {
        string AmpHtml { get; set; }

        string TemplateVersion { get; set; }
        string TemplateText { get; set; }
        List<string> Tags { get; set; }
        bool? Dkim { get; set; }

        bool RequireTls { get; set; }
        bool SkipVerification { get; set; }
        Dictionary<string, string> CustomHeaders { get; set; }

        IMailGunMessage SetAmpHtml(string ampHtml);
        IMailGunMessage SetTag(string tag);
        IMailGunMessage SetTags(List<string> tags);
        IMailGunMessage SetDkim(bool dkim);

        IMailGunMessage SetRequireTls(bool requireTls);
        IMailGunMessage SetSkipVerification(bool verification);
        IMailGunMessage AddHeader(string key, string header);
        IMailGunMessage SetTemplateVersion(string templateVersion);
        IMailGunMessage SetTemplateText(string templateText);

        Task<IEnumerable<IEmailResult>> Send(bool substitute = true, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default);

        #region Overrid Core Methods

        /// <summary>
        /// Adds from address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <returns>IEmailMessage.</returns>
        /// 
        new IMailGunMessage SetFromAddress(IEmailAddress emailAddress);
        /// <summary>
        /// Adds from address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetFromAddress(string emailAddress, string name = null);
        /// <summary>
        /// Adds to address.
        /// </summary>
        /// <param name="recipient">The address.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddToAddress(IEmailRecipient recipient);
        /// <summary>
        /// Adds to address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddToAddress(string emailAddress, string name = null);
        /// <summary>
        /// Adds to addresses.
        /// </summary>
        /// <param name="recipients">The addresses.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddToAddresses(List<IEmailRecipient> recipients);
        /// <summary>
        /// Adds the cc address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddCcAddress(IEmailAddress emailAddress);
        /// <summary>
        /// Adds the cc address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddCcAddress(string emailAddress, string name = null);
        /// <summary>
        /// Adds the cc addresses.
        /// </summary>
        /// <param name="emailAddresses">The addresses.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddCcAddresses(List<IEmailAddress> emailAddresses);
        /// <summary>
        /// Adds the BCC address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddBccAddress(IEmailAddress emailAddress);
        /// <summary>
        /// Adds the BCC address.
        /// </summary>
        /// <param name="emailAddress">The address.</param>
        /// <param name="name">The name.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddBccAddress(string emailAddress, string name = null);
        /// <summary>
        /// Adds the BCC addresses.
        /// </summary>
        /// <param name="emailAddresses">The addresses.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddBccAddresses(List<IEmailAddress> emailAddresses);
        /// <summary>
        /// Sets the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetSubject(string subject);
        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetText(string text);
        /// <summary>
        /// Sets the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetHtml(string html);
        /// <summary>
        /// Adds the attachments.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddAttachments(IEmailAttachment attachment);
        /// <summary>
        /// Adds the attachments.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddAttachments(List<IEmailAttachment> attachments);
        /// <summary>
        /// Sets the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetTemplate(string template);
        /// <summary>
        /// Sets the delivery time.
        /// </summary>
        /// <param name="deliveryTime">The delivery time.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetDeliveryTime(DateTime deliveryTime);
        /// <summary>
        /// Sets the test mode.
        /// </summary>
        /// <param name="testMode">if set to <c>true</c> [test mode].</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetTestMode(bool testMode);
        /// <summary>
        /// Sets the tracking.
        /// </summary>
        /// <param name="tracking">if set to <c>true</c> [tracking].</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetTracking(bool tracking);
        /// <summary>
        /// Sets the tracking clicks.
        /// </summary>
        /// <param name="tracking">if set to <c>true</c> [tracking].</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetTrackingClicks(bool tracking);
        /// <summary>
        /// Sets the tracking opens.
        /// </summary>
        /// <param name="tracking">if set to <c>true</c> [tracking].</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetTrackingOpens(bool tracking);
        /// <summary>
        /// Sets the reply to.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage SetReplyTo(string emailAddress);
        /// <summary>
        /// Adds the custom argument to attach data to the message (recipient custom arguments will override message level ones).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddCustomArgument(string key, string value);
        /// <summary>
        /// Adds the custom arguments.
        /// </summary>
        /// <param name="customArguments">The custom arguments.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddCustomArguments(Dictionary<string, string> customArguments);
        /// <summary>
        /// Adds the substitution to message substitutions. (recipient custom arguments will override message level ones).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddSubstitution(string key, object value);
        /// <summary>
        /// Adds the substitutions.
        /// </summary>
        /// <param name="substitutions">The substitutions.</param>
        /// <returns>IEmailMessage.</returns>
        new IMailGunMessage AddSubstitutions(Dictionary<string, object> substitutions);
        
        #endregion


    }
}