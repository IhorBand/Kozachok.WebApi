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
using Kozachok.Shared.Abstractions.Identity;

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
        IRequestHandler<ForgetPasswordCommand>,
        IRequestHandler<SendChangeEmailConfirmationCommand>,
        IRequestHandler<ActivateNewEmailCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserConfirmationCodeRepository userConfirmationCodeRepository;
        private readonly IFileRepository fileRepository;

        private readonly IUser user;

        public UserCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IUserConfirmationCodeRepository userConfirmationCodeRepository,
            IFileRepository fileRepository,
            IUser user
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.userConfirmationCodeRepository = userConfirmationCodeRepository;
            this.fileRepository = fileRepository;
            this.user = user;
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

            if (request.ThumbnailImageFileId != null && request.ThumbnailImageFileId.Value != Guid.Empty)
            {
                var file = await fileRepository.GetAsync(request.ThumbnailImageFileId.Value);
                if (file != null)
                {
                    entity.SetThumbnailImageFileId(request.ThumbnailImageFileId.Value);
                }
            }

            await userRepository.AddAsync(entity);

            var confirmationCodeStr = entity.Id.ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

            var userConfirmationCode = new UserConfirmationCode(entity.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            Commit();

            await bus.InvokeAsync(new CreateUserEvent(entity.Id, entity.Name, entity.Email, entity.Password, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if(user == null || (user != null && user.Id == null) || (user != null && user.Id != null && user.Id.Value == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var original = await userRepository.GetAsync(user.Id.Value);

            if (original == null || (original != null && original.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            original.UpdateInfo(request.Name);

            await userRepository.UpdateAsync(original);

            Commit();
            await bus.InvokeAsync(new UpdateUserEvent(original.Id, original.Name, original.Email));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            if (user == null || (user != null && user.Id == null) || (user != null && user.Id != null && user.Id.Value == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var original = await userRepository.GetAsync(user.Id.Value);

            if (original == null || (original != null && original.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
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
            if (user == null || (user != null && user.Id == null) || (user != null && user.Id != null && user.Id.Value == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var original = await userRepository.GetAsync(user.Id.Value);

            if (original == null || (original != null && original.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            await userRepository.DeleteAsync(user.Id.Value);

            Commit();
            await bus.InvokeAsync(new DeleteUserEvent(user.Id.Value));

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

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return Unit.Value;
            }

            if(currentUser.IsActive)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return Unit.Value;
            }

            var previousConfirmationCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation);

            var confirmationCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userConfirmationCode = new UserConfirmationCode(currentUser.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            if(userConfirmationCode.Id != Guid.Empty && previousConfirmationCode != null)
            {
                await userConfirmationCodeRepository.DeleteAsync(previousConfirmationCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new ResendActivationCodeEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

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

            var userConfirmationCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ConfirmationCode && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation);
            
            if (userConfirmationCode == null)
            {
                await bus.InvokeDomainNotificationAsync("Confirmation Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userConfirmationCode.UserId);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            if (currentUser.IsActive)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            currentUser.Activate();

            await userRepository.UpdateAsync(currentUser);

            await userConfirmationCodeRepository.DeleteAsync(userConfirmationCode.Id);

            Commit();
            await bus.InvokeAsync(new ActivateUserEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode));

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

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Forget Password Code.");
                return Unit.Value;
            }

            var previousCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword);
            
            var forgetPasswordCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userForgetPasswordCode = new UserConfirmationCode(currentUser.Id, forgetPasswordCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword);
            await userConfirmationCodeRepository.AddAsync(userForgetPasswordCode);

            if (userForgetPasswordCode.Id != Guid.Empty && previousCode != null)
            {
                await userConfirmationCodeRepository.DeleteAsync(previousCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new SendForgetPasswordEmailEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode)).ConfigureAwait(false);

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

            var userForgetPasswordCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ForgetPasswordCode && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword);

            if (userForgetPasswordCode == null)
            {
                await bus.InvokeDomainNotificationAsync("Forget Password Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userForgetPasswordCode.UserId);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Change Password for that User.");
                return Unit.Value;
            }

            currentUser.ChangePassword(request.Password);

            await userRepository.UpdateAsync(currentUser);

            await userConfirmationCodeRepository.DeleteAsync(userForgetPasswordCode.Id);

            Commit();
            await bus.InvokeAsync(new ForgetPasswordEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<Unit> Handle(SendChangeEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            if (user == null || (user != null && user.Id == null) || (user != null && user.Id != null && user.Id.Value == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not Authorized.");
                return Unit.Value;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .Is(e => userRepository.AnyAsync(u => u.Email == e.Email).Result, async () => await bus.InvokeDomainNotificationAsync("E-mail already exists."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var previousCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail);

            var changeEmailCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userChangeEmailCode = new UserConfirmationCode(currentUser.Id, changeEmailCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail);
            userChangeEmailCode.SetAdditionalData(request.Email);
            await userConfirmationCodeRepository.AddAsync(userChangeEmailCode);

            if (userChangeEmailCode.Id != Guid.Empty && previousCode != null)
            {
                await userConfirmationCodeRepository.DeleteAsync(previousCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new SendChangeEmailConfirmationEvent(currentUser.Id, currentUser.Name, request.Email, userChangeEmailCode.ConfirmationCode)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ActivateNewEmailCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ChangeEmailCode, async () => await bus.InvokeDomainNotificationAsync("Please, provide Forget Password Code."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userChangeEmailCode = await userConfirmationCodeRepository.FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ChangeEmailCode && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail);

            if (userChangeEmailCode == null || (userChangeEmailCode != null && userChangeEmailCode.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Change Email Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userChangeEmailCode.UserId);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Change Password for that User.");
                return Unit.Value;
            }

            currentUser.ChangeEmail(userChangeEmailCode.AdditionalData);

            await userRepository.UpdateAsync(currentUser);

            await userConfirmationCodeRepository.DeleteAsync(userChangeEmailCode.Id);

            Commit();

            return Unit.Value;
        }

        public void Dispose() => userRepository.Dispose();
    }
}
