using Kozachok.Domain.Commands.User;
using Kozachok.Domain.Events.User;
using Kozachok.Domain.Handlers.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Utils.Validation;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.DTO.Common;
using System;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.DTO.Models.DbEntities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Kozachok.Shared.DTO.Models.Result.Email;

namespace Kozachok.Domain.Handlers.Commands
{
    public class UserCommandHandler : 
        CommandHandler, 
        IRequestHandler<CreateUserCommand, EmailTimer>, 
        IRequestHandler<UpdateUserCommand>, 
        IRequestHandler<ChangeUserPasswordCommand>, 
        IRequestHandler<DeleteUserCommand>,
        IRequestHandler<ResendActivationCodeCommand, EmailTimer>,
        IRequestHandler<ActivateUserCommand>,
        IRequestHandler<SendForgetPasswordEmailCommand, EmailTimer>,
        IRequestHandler<ForgetPasswordCommand>,
        IRequestHandler<SendChangeEmailConfirmationCommand, EmailTimer>,
        IRequestHandler<ActivateNewEmailCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserConfirmationCodeRepository userConfirmationCodeRepository;

        private readonly IUser user;

        public UserCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IUserConfirmationCodeRepository userConfirmationCodeRepository,
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
            this.user = user;
        }

        public async Task<EmailTimer> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            request
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid e-mail."))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password."))
                .IsMatchMaxLength(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Name is too big."))
                .IsMatchMaxLength(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Email is too big."))
                .IsMatchMaxLength(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Password is too big."))
                .IsMatchMaxLength(e => e.PasswordConfirmation, async () => await bus.InvokeDomainNotificationAsync("Password Confirmation is too big."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password confirmation."))
                .Is(e => userRepository.AnyAsync(u => u.Email == request.Email).Result, async () => await bus.InvokeDomainNotificationAsync("E-mail already exists."));

            if (!IsValidOperation())
            {
                return null;
            }

            var entity = new User(request.Name, request.Email, request.Password, false);

            await userRepository.AddAsync(entity);

            var confirmationCodeStr = entity.Id.ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

            var nextAttemptDate = DateTime.UtcNow.AddMinutes(5);

            var userConfirmationCode = new UserConfirmationCode(entity.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation, 1, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            Commit();

            await bus.InvokeAsync(new CreateUserEvent(entity.Id, entity.Name, entity.Email, entity.Password, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = 1, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
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
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await bus.InvokeDomainNotificationAsync("Name is too big."));

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
            if (IsUserAuthorized(user) == false)
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
                .Is(e => e.PasswordConfirmation != e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password confirmation."))
                .IsMatchMaxLength(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Password is too big."))
                .IsMatchMaxLength(e => e.PasswordConfirmation, async () => await bus.InvokeDomainNotificationAsync("PasswordConfirmation is too big."))
                .Is(e => !original.CheckPassword(e.OldPassword), async () => await bus.InvokeDomainNotificationAsync("Old Password is incorrect."));

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
            if (IsUserAuthorized(user) == false)
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

        public async Task<EmailTimer> Handle(ResendActivationCodeCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("E-mail is too big."));
            
            if (!IsValidOperation())
            {
                return null;
            }

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return null;
            }

            if(currentUser.IsActive)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return null;
            }

            var previousConfirmationCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation)
                .OrderByDescending(ucc => ucc.CreatedDateUTC)
                .FirstOrDefaultAsync();

            byte attemptNum = 1;
            if(previousConfirmationCode != null && previousConfirmationCode.Id != Guid.Empty)
            {
                if (previousConfirmationCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await bus.InvokeDomainNotificationAsync($"Please, wait {(DateTime.UtcNow - previousConfirmationCode.NextAttemptDate)}.");
                    return null;
                }

                attemptNum = previousConfirmationCode.NumberOfAttempt;
                attemptNum += 1;

                if (attemptNum > 3 || (DateTime.UtcNow - previousConfirmationCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            DateTime nextAttemptDate = DateTime.UtcNow;
            if (attemptNum < 3)
            {
                nextAttemptDate = nextAttemptDate.AddMinutes(5);
            }
            else
            {
                nextAttemptDate = nextAttemptDate.AddDays(1);
            }

            var confirmationCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userConfirmationCode = new UserConfirmationCode(currentUser.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation, attemptNum, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode);

            Commit();
            await bus.InvokeAsync(new ResendActivationCodeEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
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

            var oldUserConfirmationCodes = await userConfirmationCodeRepository.Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation).ToListAsync();

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                await userConfirmationCodeRepository.DeleteAsync(oldUserConfirmationCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new ActivateUserEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<EmailTimer> Handle(SendForgetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("E-mail is too big."));

            if (!IsValidOperation())
            {
                return null;
            }

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Send Forget Password Code.");
                return null;
            }

            var previousCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword)
                .OrderByDescending(ucc => ucc.CreatedDateUTC)
                .FirstOrDefaultAsync();

            byte attemptNum = 1;
            if (previousCode != null && previousCode.Id != Guid.Empty)
            {
                if (previousCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await bus.InvokeDomainNotificationAsync($"Please, wait {(DateTime.UtcNow - previousCode.NextAttemptDate)}.");
                    return null;
                }

                attemptNum = previousCode.NumberOfAttempt;
                attemptNum += 1;


                if (attemptNum > 3 || (DateTime.UtcNow - previousCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            DateTime nextAttemptDate = DateTime.UtcNow;
            if (attemptNum < 3)
            {
                nextAttemptDate = nextAttemptDate.AddMinutes(5);
            }
            else
            {
                nextAttemptDate = nextAttemptDate.AddDays(1);
            }
            
            var forgetPasswordCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userForgetPasswordCode = new UserConfirmationCode(currentUser.Id, forgetPasswordCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword, attemptNum, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userForgetPasswordCode);

            Commit();
            await bus.InvokeAsync(new SendForgetPasswordEmailEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ForgetPasswordCode, async () => await bus.InvokeDomainNotificationAsync("Please, provide Forget Password Code."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Invalid password."))
                .IsMatchMaxLength(e => e.Password, async () => await bus.InvokeDomainNotificationAsync("Password is too big."))
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

            var oldUserConfirmationCodes = await userConfirmationCodeRepository.Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword).ToListAsync();

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                await userConfirmationCodeRepository.DeleteAsync(oldUserConfirmationCode.Id);
            }

            Commit();
            await bus.InvokeAsync(new ForgetPasswordEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<EmailTimer> Handle(SendChangeEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not Authorized.");
                return null;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await bus.InvokeDomainNotificationAsync("E-mail is too big."))
                .Is(e => userRepository.AnyAsync(u => u.Email == e.Email).Result, async () => await bus.InvokeDomainNotificationAsync("E-mail already exists."))
                .Is(e => e.Email == user.Email, async () => await bus.InvokeDomainNotificationAsync("Please, provide new e-mail."));

            if (!IsValidOperation())
            {
                return null;
            }

            var previousCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail)
                .OrderByDescending(ucc => ucc.CreatedDateUTC)
                .FirstOrDefaultAsync();

            byte attemptNum = 1;
            if (previousCode != null && previousCode.Id != Guid.Empty)
            {
                if (previousCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await bus.InvokeDomainNotificationAsync($"Please, wait {(DateTime.UtcNow - previousCode.NextAttemptDate)}.");
                    return null;
                }

                attemptNum = previousCode.NumberOfAttempt;
                attemptNum += 1;

                if (attemptNum > 3 || (DateTime.UtcNow - previousCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            DateTime nextAttemptDate = DateTime.UtcNow;
            if (attemptNum < 3)
            {
                nextAttemptDate = nextAttemptDate.AddMinutes(5);
            }
            else
            {
                nextAttemptDate = nextAttemptDate.AddDays(1);
            }

            var changeEmailCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userChangeEmailCode = new UserConfirmationCode(currentUser.Id, changeEmailCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail, attemptNum, nextAttemptDate);
            userChangeEmailCode.SetAdditionalData(request.Email);
            await userConfirmationCodeRepository.AddAsync(userChangeEmailCode);

            Commit();
            await bus.InvokeAsync(new SendChangeEmailConfirmationEvent(currentUser.Id, currentUser.Name, request.Email, userChangeEmailCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
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

            var oldUserConfirmationCodes = await userConfirmationCodeRepository.Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail).ToListAsync();

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                await userConfirmationCodeRepository.DeleteAsync(oldUserConfirmationCode.Id);
            }

            Commit();

            return Unit.Value;
        }

        public void Dispose() => userRepository.Dispose();
    }
}
