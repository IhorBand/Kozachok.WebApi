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

        public UserEventHandler(
            EmailService emailService,
            EndpointsConfiguration endpointsConfiguration)
        {
            this.emailService = emailService;
            this.endpointsConfiguration = endpointsConfiguration;
        }

        public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
        {
            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            var parameters = new Dictionary<string, string>()
            {
                {EmailParameters.UserName, notification.Name},
                {EmailParameters.ConfirmationEmailUrl, confirmationUrl}
            };

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailConfirmation, parameters);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailConfirmation, parameters);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return Task.CompletedTask;
        }
        
        public Task Handle(ActivateUserEvent notification, CancellationToken cancellationToken)
        {
            //TODO: Send Welcome email.
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.PasswordReset, parameters);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            emailService.SendEmailTemplateAsync(notification.Email, EmailTemplates.EmailReset, parameters);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return Task.CompletedTask;
        }
    }
}
