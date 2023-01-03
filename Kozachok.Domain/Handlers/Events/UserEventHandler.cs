using Kozachok.Domain.Events.User;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.Domain.Emails;

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

            string htmlContent = $"<html><body><h1>Hello, {notification.Name}! Please, find your confirmation URL below. \n {confirmationUrl} \n\n Thank you.</h1></body></html>";

            emailService.SendEmailAsync(notification.Email, htmlContent, "Confirmation Email");

            return Task.CompletedTask;
        }

        public Task Handle(UpdateUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(DeleteUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ChangeUserPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ResendActivationCodeEvent notification, CancellationToken cancellationToken)
        {

            var confirmationUrl = endpointsConfiguration.ConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            string htmlContent = $"<html><body><h1>Hello, {notification.Name}! Please, find your new confirmation URL below. \n {confirmationUrl} \n\n Thank you.</h1></body></html>";

            emailService.SendEmailAsync(notification.Email, htmlContent, "Resend Confirmation Email");

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

            string htmlContent = $"<html><body><h1>Hello, {notification.Name}! Please, find your forget password URL below. \n {forgetPasswordUrl} \n\n Thank you.</h1></body></html>";

            emailService.SendEmailAsync(notification.Email, htmlContent, "Forget Password");

            return Task.CompletedTask;
        }

        public Task Handle(ForgetPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(SendChangeEmailConfirmationEvent notification, CancellationToken cancellationToken)
        {
            var changeEmailConfirmationUrl = endpointsConfiguration.ChangeEmailConfirmationUrl.Replace("code=", $"code={notification.ConfirmationCode}");

            string htmlContent = $"<html><body><h1>Hello, {notification.Name}! Please, find your Change Email URL below. \n {changeEmailConfirmationUrl} \n\n Thank you.</h1></body></html>";

            emailService.SendEmailAsync(notification.Email, htmlContent, "Change Email");

            return Task.CompletedTask;
        }
    }
}
