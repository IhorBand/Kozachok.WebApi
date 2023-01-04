using Kozachok.Domain.Events.User;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.Domain.Emails;
using System.Collections.Generic;
using Kozachok.Shared.DTO.Email;

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
        private readonly EmailService emailService;
        private readonly EndpointsConfiguration endpointsConfiguration;
        private readonly MailConfiguration mailConfiguration;

        public UserEventHandler(
            EmailService emailService,
            EndpointsConfiguration endpointsConfiguration,
            MailConfiguration mailConfiguration)
        {
            this.emailService = emailService;
            this.endpointsConfiguration = endpointsConfiguration;
            this.mailConfiguration = mailConfiguration;
        }

        public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
        {
            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            var parameters = new Dictionary<string, string>()
            {
                {EmailParameters.UserName, notification.Name},
                {EmailParameters.ConfirmationEmailUrl, confirmationUrl}
            };

            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailConfirmation, parameters);

            return Task.CompletedTask;
        }

        public Task Handle(UpdateUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(DeleteUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ChangeUserPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ResendActivationCodeEvent notification, CancellationToken cancellationToken)
        {

            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            var parameters = new Dictionary<string, string>()
            {
                {EmailParameters.UserName, notification.Name},
                {EmailParameters.ConfirmationEmailUrl, confirmationUrl}
            };

            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailConfirmation, parameters);

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

            var parameters = new Dictionary<string, string>()
            {
                {EmailParameters.UserName, notification.Name},
                {EmailParameters.ConfirmationEmailUrl, forgetPasswordUrl}
            };

            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.PasswordReset, parameters);

            return Task.CompletedTask;
        }

        public Task Handle(ForgetPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(SendChangeEmailConfirmationEvent notification, CancellationToken cancellationToken)
        {
            var changeEmailConfirmationUrl = endpointsConfiguration.ChangeEmailConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            var parameters = new Dictionary<string, string>()
            {
                {EmailParameters.UserName, notification.Name},
                {EmailParameters.ConfirmationEmailUrl, changeEmailConfirmationUrl}
            };

            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailReset, parameters);

            return Task.CompletedTask;
        }
    }
}
