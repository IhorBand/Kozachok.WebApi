using Kozachok.Domain.Commands.User;
using Kozachok.Domain.Events.User;
using Kozachok.Domain.Handlers.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Utils.Validation;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using System;

namespace Kozachok.Domain.Handlers.Commands
{
    public class UserCommandHandler : 
        CommandHandler, 
        IRequestHandler<CreateUserCommand>, 
        IRequestHandler<UpdateUserCommand>, 
        IRequestHandler<ChangeUserPasswordCommand>, 
        IRequestHandler<DeleteUserCommand>,
        IRequestHandler<ResendActivationCodeCommand>,
        IRequestHandler<ActivateUserCommand>,
        IRequestHandler<SendForgetPasswordEmailCommand>,
        IRequestHandler<ForgetPasswordCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserConfirmationCodeRepository userConfirmationCodeRepository;
        private readonly IUserForgetPasswordCodeRepository userForgetPasswordCodeRepository;

        public UserCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IUserConfirmationCodeRepository userConfirmationCodeRepository,
            IUserForgetPasswordCodeRepository userForgetPasswordCodeRepository
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.userConfirmationCodeRepository = userConfirmationCodeRepository;
            this.userForgetPasswordCodeRepository = userForgetPasswordCodeRepository;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            request
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid e-mail."))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password confirmation."))
                .Is(e => userRepository.AnyAsync(u => u.Email == request.Email).Result, async () => await bus.InvokeDomainNotificationAsync("E-mail already exists."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var entity = new User(request.Name, request.Email, request.Password, false);
            await userRepository.AddAsync(entity);

            var confirmationCodeStr = entity.Id.ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

            var userConfirmationCode = new UserConfirmationCode(entity.Id, confirmationCodeStr);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            Commit();

            _ = bus.InvokeAsync(new CreateUserEvent(entity.Id, entity.Name, entity.Email, entity.Password, userConfirmationCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var original = await userRepository.GetAsync(request.Id);

            if (original == null)
            {
                await bus.InvokeDomainNotificationAsync("Not found.");
                return Unit.Value;
            }

            request
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid e-mail."))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .Is(e => userRepository.AnyAsync(u => u.Email == request.Email && u.Id != request.Id).Result, async () => await bus.InvokeDomainNotificationAsync("E-mail already exists."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            original.UpdateInfo(request.Name, request.Email);

            await userRepository.UpdateAsync(original);

            Commit();
            await bus.InvokeAsync(new UpdateUserEvent(original.Id, original.Name, original.Email));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var original = await userRepository.GetAsync(request.Id);

            if (original == null)
            {
                await bus.InvokeDomainNotificationAsync("Not found.");
                return Unit.Value;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password confirmation."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            original.ChangePassword(request.Password);

            await userRepository.UpdateAsync(original);

            Commit();
            await bus.InvokeAsync(new ChangeUserPasswordEvent(original.Id, original.Password));

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await userRepository.DeleteAsync(request.Id);

            Commit();
            await bus.InvokeAsync(new DeleteUserEvent(request.Id));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ResendActivationCodeCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."));
            
            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var user = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return Unit.Value;
            }

            if(user.IsActive)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return Unit.Value;
            }

            var previousConfirmationCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.UserId == user.Id);

            var confirmationCodeStr = Guid.NewGuid().ToString().Replace("-", "") + user.Id.ToString().Replace("-", "");

            var userConfirmationCode = new UserConfirmationCode(user.Id, confirmationCodeStr);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            if(userConfirmationCode.Id != Guid.Empty && previousConfirmationCode != null)
            {
                await userConfirmationCodeRepository.DeleteAsync(previousConfirmationCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new ResendActivationCodeEvent(user.Id, user.Name, user.Email, userConfirmationCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ConfirmationCode, async () => await bus.InvokeDomainNotificationAsync("Please, provide Confirmation Code."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userConfirmationCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ConfirmationCode);
            
            if (userConfirmationCode == null)
            {
                await bus.InvokeDomainNotificationAsync("Confirmation Code is Invalid.");
                return Unit.Value;
            }

            var user = await userRepository.GetAsync(userConfirmationCode.UserId);

            if (user == null)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            if (user.IsActive)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            user.Activate();

            await userRepository.UpdateAsync(user);

            await userConfirmationCodeRepository.DeleteAsync(userConfirmationCode.Id);

            Commit();
            await bus.InvokeAsync(new ActivateUserEvent(user.Id, user.Name, user.Email, userConfirmationCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<Unit> Handle(SendForgetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var user = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Forget Password Code.");
                return Unit.Value;
            }

            var previousCode = await userForgetPasswordCodeRepository.FirstOrDefaultAsync(ucc => ucc.UserId == user.Id);
            
            var forgetPasswordCodeStr = Guid.NewGuid().ToString().Replace("-", "") + user.Id.ToString().Replace("-", "");

            var userForgetPasswordCode = new UserForgetPasswordCode(user.Id, forgetPasswordCodeStr);
            await userForgetPasswordCodeRepository.AddAsync(userForgetPasswordCode);

            if (userForgetPasswordCode.Id != Guid.Empty && previousCode != null)
            {
                await userForgetPasswordCodeRepository.DeleteAsync(previousCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new SendForgetPasswordEmailEvent(user.Id, user.Name, user.Email, userForgetPasswordCode.ForgetPasswordCode));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ForgetPasswordCode, async () => await bus.InvokeDomainNotificationAsync("Please, provide Forget Password Code."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password confirmation."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userForgetPasswordCode = await userForgetPasswordCodeRepository.FirstOrDefaultAsync(ucc => ucc.ForgetPasswordCode == request.ForgetPasswordCode);

            if (userForgetPasswordCode == null)
            {
                await bus.InvokeDomainNotificationAsync("Forget Password Code is Invalid.");
                return Unit.Value;
            }

            var user = await userRepository.GetAsync(userForgetPasswordCode.UserId);

            if (user == null)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Change Password for that User.");
                return Unit.Value;
            }

            user.ChangePassword(request.Password);

            await userRepository.UpdateAsync(user);

            await userForgetPasswordCodeRepository.DeleteAsync(userForgetPasswordCode.Id);

            Commit();
            await bus.InvokeAsync(new ForgetPasswordEvent(user.Id, user.Name, user.Email, userForgetPasswordCode.ForgetPasswordCode));

            return Unit.Value;
        }

        public void Dispose() => userRepository.Dispose();
    }
}
