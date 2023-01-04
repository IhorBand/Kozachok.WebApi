using Kozachok.Shared.DTO.Configuration;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Kozachok.Domain.Emails
{
    public class EmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly MailConfiguration mailConfiguration;

        public EmailService(
            ILogger<EmailService> logger,
            MailConfiguration mailConfiguration) 
        {
            this.logger = logger;
            this.mailConfiguration = mailConfiguration;
        }

        public async System.Threading.Tasks.Task SendEmailTemplateAsync(string toEmail, int templateId, Dictionary<string,string> parameters)
        {
            var conf = new Configuration();
            conf.ApiKey.Add("api-key", mailConfiguration.ApiKey);

            var apiInstance = new TransactionalEmailsApi(conf);
            SendSmtpEmailSender Email = new SendSmtpEmailSender(mailConfiguration.FromName, mailConfiguration.FromEmail);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>
            {
                new SendSmtpEmailTo(toEmail, null)
            };

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, null, null, null, null, null, null, templateId, parameters);
                CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
                logger.LogInformation("Email sent successfullty.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while sending email.");
            }
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string toEmail, string htmlContent, string subject)
        {
            var conf = new Configuration();
            conf.ApiKey.Add("api-key", mailConfiguration.ApiKey);

            var apiInstance = new TransactionalEmailsApi(conf);
            SendSmtpEmailSender Email = new SendSmtpEmailSender(mailConfiguration.FromName, mailConfiguration.FromEmail);
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(toEmail, null);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>
            {
                smtpEmailTo
            };

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, htmlContent, null, subject);
                CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
                logger.LogInformation("Email sent successfullty.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while sending email.");
            }
        }
    }
}
