using Kozachok.Domain.Events.User;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using Kozachok.Shared.DTO.Configuration;
using System;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;
using Org.BouncyCastle.Asn1.Esf;
using Kozachok.Domain.Commands.User;

namespace Kozachok.Domain.Handlers.Events
{
    public class UserEventHandler : 
        INotificationHandler<CreateUserEvent>, 
        INotificationHandler<UpdateUserEvent>, 
        INotificationHandler<DeleteUserEvent>, 
        INotificationHandler<ChangeUserPasswordEvent>,
        INotificationHandler<ResendActivationCodeEvent>,
        INotificationHandler<ActivateUserEvent>,
        INotificationHandler<SendForgetPasswordEmailEvent>,
        INotificationHandler<ForgetPasswordEvent>,
        INotificationHandler<SendChangeEmailConfirmationEvent>
    {
        private readonly MailConfiguration mailConfiguration;
        private readonly EndpointsConfiguration endpointsConfiguration;

        public UserEventHandler(MailConfiguration mailConfiguration,
            EndpointsConfiguration endpointsConfiguration)
        {
            this.mailConfiguration = mailConfiguration;
            this.endpointsConfiguration = endpointsConfiguration;
        }

        public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
        {
            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            //TODO: Move Email Jobs to another class
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(mailConfiguration.FromName, mailConfiguration.FromEmail));
            mailMessage.To.Add(new MailboxAddress(notification.Name, notification.Email));
            mailMessage.Subject = "Confirmation Email";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello, {notification.Name}! Please, find your confirmation URL below. \n {confirmationUrl} \n\n Thank you."
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(mailConfiguration.SmtpHost, mailConfiguration.SmtpPort, false);
                smtpClient.Authenticate(mailConfiguration.UserName, mailConfiguration.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }

        public Task Handle(UpdateUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(DeleteUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ChangeUserPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ResendActivationCodeEvent notification, CancellationToken cancellationToken)
        {
            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            //TODO: Move Email Jobs to another class
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(mailConfiguration.FromName, mailConfiguration.FromEmail));
            mailMessage.To.Add(new MailboxAddress(notification.Name, notification.Email));
            mailMessage.Subject = "Confirmation Email";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello, {notification.Name}! Please, find your new confirmation URL below. \n {confirmationUrl} \n\n Thank you."
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(mailConfiguration.SmtpHost, mailConfiguration.SmtpPort, false);
                smtpClient.Authenticate(mailConfiguration.UserName, mailConfiguration.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }
        
        public Task Handle(ActivateUserEvent notification, CancellationToken cancellationToken)
        {
            //TODO: Semd Welcome email.
            return Task.CompletedTask;
        }

        public Task Handle(SendForgetPasswordEmailEvent notification, CancellationToken cancellationToken)
        {
            var forgetPasswordUrl = endpointsConfiguration.ForgetPasswordUrl.Replace("code=", $"code={notification.ForgetPasswordCode}");

            //TODO: Move Email Jobs to another class
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(mailConfiguration.FromName, mailConfiguration.FromEmail));
            mailMessage.To.Add(new MailboxAddress(notification.Name, notification.Email));
            mailMessage.Subject = "Forget Password Email";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello, {notification.Name}! Please, find your forget password URL below. \n {forgetPasswordUrl} \n\n Thank you."
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(mailConfiguration.SmtpHost, mailConfiguration.SmtpPort, false);
                smtpClient.Authenticate(mailConfiguration.UserName, mailConfiguration.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }

        public Task Handle(ForgetPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(SendChangeEmailConfirmationEvent notification, CancellationToken cancellationToken)
        {
            var changeEmailConfirmationUrl = endpointsConfiguration.ChangeEmailConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            //TODO: Move Email Jobs to another class
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(mailConfiguration.FromName, mailConfiguration.FromEmail));
            mailMessage.To.Add(new MailboxAddress(notification.Name, notification.Email));
            mailMessage.Subject = "Forget Password Email";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Hello, {notification.Name}! Please, find your Change Email URL below. \n {changeEmailConfirmationUrl} \n\n Thank you."
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(mailConfiguration.SmtpHost, mailConfiguration.SmtpPort, false);
                smtpClient.Authenticate(mailConfiguration.UserName, mailConfiguration.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}
