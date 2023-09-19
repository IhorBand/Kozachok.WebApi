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
                .IsInvalidEmail(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Invalid e-mail."))
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password."))
                .IsMatchMaxLength(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Name is too big."))
                .IsMatchMaxLength(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Email is too big."))
                .IsMatchMaxLength(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Password is too big."))
                .IsMatchMaxLength(e => e.PasswordConfirmation, async () => await Bus.InvokeDomainNotificationAsync("Password Confirmation is too big."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password confirmation."))
                .Is(e => userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken).Result, async () => await Bus.InvokeDomainNotificationAsync("E-mail already exists."));

            if (!IsValidOperation())
            {
                return null;
            }

            var isEmailAlreadyExist = await userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (isEmailAlreadyExist)
            {
                await Bus.InvokeDomainNotificationAsync("Email already exist.");
                return null;
            }

            var entity = User.Create(request.Name, request.Email, request.Password, false);

            await userRepository.AddAsync(entity, cancellationToken);

            var confirmationCodeStr = entity.Id.ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

            var nextAttemptDate = DateTime.UtcNow.AddMinutes(5);

            var userConfirmationCode = UserConfirmationCode.Create(entity.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation, 1, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode, cancellationToken);

            Commit();

            await Bus.InvokeAsync(new CreateUserEvent(entity.Id, entity.Name, entity.Email, entity.Password, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = 1, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var original = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (original == null || original.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Invalid name."))
                .IsMatchMaxLength(e => e.Name, async () => await Bus.InvokeDomainNotificationAsync("Name is too big."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            original.UpdateInfo(request.Name);

            userRepository.Update(original);

            Commit();
            await Bus.InvokeAsync(new UpdateUserEvent(original.Id, original.Name, original.Email));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var original = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (original == null || original.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password confirmation."))
                .IsMatchMaxLength(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Password is too big."))
                .IsMatchMaxLength(e => e.PasswordConfirmation, async () => await Bus.InvokeDomainNotificationAsync("PasswordConfirmation is too big."))
                .Is(e => !original.CheckPassword(e.OldPassword), async () => await Bus.InvokeDomainNotificationAsync("Old Password is incorrect."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            original.ChangePassword(request.Password);

            userRepository.Update(original);

            Commit();
            await Bus.InvokeAsync(new ChangeUserPasswordEvent(original.Id, original.Password));

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            var original = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (original == null || original.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            userRepository.Delete(original);

            Commit();

            await Bus.InvokeAsync(new DeleteUserEvent(user.Id.Value));

            return Unit.Value;
        }

        public async Task<EmailTimer> Handle(ResendActivationCodeCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("E-mail is too big."));
            
            if (!IsValidOperation())
            {
                return null;
            }

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return null;
            }

            if(currentUser.IsActive)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Send Activation Code.");
                return null;
            }

            var previousConfirmationCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation)
                .OrderByDescending(ucc => ucc.CreatedDateUtc)
                .FirstOrDefaultAsync(cancellationToken);

            byte attemptNum = 1;
            if(previousConfirmationCode != null && previousConfirmationCode.Id != Guid.Empty)
            {
                if (previousConfirmationCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await Bus.InvokeDomainNotificationAsync($"Please, wait {(previousConfirmationCode.NextAttemptDate - DateTime.UtcNow)}.");
                    return null;
                }

                attemptNum = previousConfirmationCode.NumberOfAttempt;
                attemptNum += 1;

                if (attemptNum > 3 || (DateTime.UtcNow - previousConfirmationCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            var nextAttemptDate = DateTime.UtcNow;
            if (attemptNum < 3)
            {
                nextAttemptDate = nextAttemptDate.AddMinutes(5);
            }
            else
            {
                nextAttemptDate = nextAttemptDate.AddDays(1);
            }

            var confirmationCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userConfirmationCode = UserConfirmationCode.Create(currentUser.Id, confirmationCodeStr, Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation, attemptNum, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userConfirmationCode, cancellationToken);

            Commit();
            await Bus.InvokeAsync(new ResendActivationCodeEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ConfirmationCode, async () => await Bus.InvokeDomainNotificationAsync("Please, provide Confirmation Code."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userConfirmationCode = await userConfirmationCodeRepository
                .FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ConfirmationCode 
                                            && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation, cancellationToken);
            
            if (userConfirmationCode == null)
            {
                await Bus.InvokeDomainNotificationAsync("Confirmation Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userConfirmationCode.UserId, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            if (currentUser.IsActive)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Activate User.");
                return Unit.Value;
            }

            currentUser.Activate();

            userRepository.Update(currentUser);

            var oldUserConfirmationCodes = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id 
                              && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.EmailConfirmation)
                .ToListAsync(cancellationToken);

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                userConfirmationCodeRepository.Delete(oldUserConfirmationCode);
            }

            Commit();
            await Bus.InvokeAsync(new ActivateUserEvent(currentUser.Id, currentUser.Name, currentUser.Email, userConfirmationCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<EmailTimer> Handle(SendForgetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("E-mail is too big."));

            if (!IsValidOperation())
            {
                return null;
            }

            var currentUser = await userRepository.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Send Forget Password Code.");
                return null;
            }

            var previousCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword)
                .OrderByDescending(ucc => ucc.CreatedDateUtc)
                .FirstOrDefaultAsync(cancellationToken);

            byte attemptNum = 1;
            if (previousCode != null && previousCode.Id != Guid.Empty)
            {
                if (previousCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await Bus.InvokeDomainNotificationAsync($"Please, wait {(previousCode.NextAttemptDate - DateTime.UtcNow)}.");
                    return null;
                }

                attemptNum = previousCode.NumberOfAttempt;
                attemptNum += 1;


                if (attemptNum > 3 || (DateTime.UtcNow - previousCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            var nextAttemptDate = DateTime.UtcNow;

            nextAttemptDate = attemptNum < 3 ? nextAttemptDate.AddMinutes(5) : nextAttemptDate.AddDays(1);
            
            var forgetPasswordCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userForgetPasswordCode = UserConfirmationCode.Create(currentUser.Id, forgetPasswordCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword, attemptNum, nextAttemptDate);
            await userConfirmationCodeRepository.AddAsync(userForgetPasswordCode, cancellationToken);

            Commit();
            await Bus.InvokeAsync(new SendForgetPasswordEmailEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ForgetPasswordCode, async () => await Bus.InvokeDomainNotificationAsync("Please, provide Forget Password Code."))
                .IsNullEmptyOrWhitespace(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password."))
                .IsMatchMaxLength(e => e.Password, async () => await Bus.InvokeDomainNotificationAsync("Password is too big."))
                .Is(e => e.PasswordConfirmation != e.Password, async () => await Bus.InvokeDomainNotificationAsync("Invalid password confirmation."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userForgetPasswordCode = await userConfirmationCodeRepository
                .FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ForgetPasswordCode 
                                            && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword, cancellationToken);

            if (userForgetPasswordCode == null)
            {
                await Bus.InvokeDomainNotificationAsync("Forget Password Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userForgetPasswordCode.UserId, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Change Password for that User.");
                return Unit.Value;
            }

            currentUser.ChangePassword(request.Password);

            userRepository.Update(currentUser);

            var oldUserConfirmationCodes = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id 
                              && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ForgotPassword)
                .ToListAsync(cancellationToken);

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                await userConfirmationCodeRepository.DeleteAsync(oldUserConfirmationCode.Id, cancellationToken);
            }

            Commit();
            await Bus.InvokeAsync(new ForgetPasswordEvent(currentUser.Id, currentUser.Name, currentUser.Email, userForgetPasswordCode.ConfirmationCode));

            return Unit.Value;
        }

        public async Task<EmailTimer> Handle(SendChangeEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return null;
            }

            var currentUser = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not Authorized.");
                return null;
            }

            request
                .IsNullEmptyOrWhitespace(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Please, provide E-mail."))
                .IsInvalidEmail(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("Invalid E-mail."))
                .IsMatchMaxLength(e => e.Email, async () => await Bus.InvokeDomainNotificationAsync("E-mail is too big."))
                .Is(e => userRepository.AnyAsync(u => u.Email == e.Email, cancellationToken).Result, async () => await Bus.InvokeDomainNotificationAsync("E-mail already exists."))
                .Is(e => e.Email == user.Email, async () => await Bus.InvokeDomainNotificationAsync("Please, provide new e-mail."));

            if (!IsValidOperation())
            {
                return null;
            }

            var previousCode = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail)
                .OrderByDescending(ucc => ucc.CreatedDateUtc)
                .FirstOrDefaultAsync(cancellationToken);

            byte attemptNum = 1;
            if (previousCode != null && previousCode.Id != Guid.Empty)
            {
                if (previousCode.NextAttemptDate > DateTime.UtcNow)
                {
                    await Bus.InvokeDomainNotificationAsync($"Please, wait {(previousCode.NextAttemptDate - DateTime.UtcNow)}.");
                    return null;
                }

                attemptNum = previousCode.NumberOfAttempt;
                attemptNum += 1;

                if (attemptNum > 3 || (DateTime.UtcNow - previousCode.NextAttemptDate).Days >= 1)
                {
                    attemptNum = 1;
                }
            }

            var nextAttemptDate = DateTime.UtcNow;

            nextAttemptDate = attemptNum < 3 ? nextAttemptDate.AddMinutes(5) : nextAttemptDate.AddDays(1);

            var changeEmailCodeStr = Guid.NewGuid().ToString().Replace("-", "") + currentUser.Id.ToString().Replace("-", "");

            var userChangeEmailCode = UserConfirmationCode.Create(currentUser.Id, changeEmailCodeStr, Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail, attemptNum, nextAttemptDate);
            userChangeEmailCode.SetAdditionalData(request.Email);
            await userConfirmationCodeRepository.AddAsync(userChangeEmailCode, cancellationToken);

            Commit();
            await Bus.InvokeAsync(new SendChangeEmailConfirmationEvent(currentUser.Id, currentUser.Name, request.Email, userChangeEmailCode.ConfirmationCode)).ConfigureAwait(false);

            return new EmailTimer() { AttemtNumber = attemptNum, NextAttemptDate = nextAttemptDate };
        }

        public async Task<Unit> Handle(ActivateNewEmailCommand request, CancellationToken cancellationToken)
        {
            request
                .IsNullEmptyOrWhitespace(e => e.ChangeEmailCode, async () => await Bus.InvokeDomainNotificationAsync("Please, provide Forget Password Code."));

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var userChangeEmailCode = await userConfirmationCodeRepository
                .FirstOrDefaultAsync(ucc => ucc.ConfirmationCode == request.ChangeEmailCode 
                                            && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail, cancellationToken);

            if (userChangeEmailCode == null || userChangeEmailCode.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Change Email Code is Invalid.");
                return Unit.Value;
            }

            var currentUser = await userRepository.GetAsync(userChangeEmailCode.UserId, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Change Password for that User.");
                return Unit.Value;
            }

            currentUser.ChangeEmail(userChangeEmailCode.AdditionalData);

            userRepository.Update(currentUser);

            var oldUserConfirmationCodes = await userConfirmationCodeRepository
                .Query(ucc => ucc.UserId == currentUser.Id && ucc.CodeType == Shared.DTO.Enums.ConfirmationCodeType.ChangeEmail)
                .ToListAsync(cancellationToken);

            foreach (var oldUserConfirmationCode in oldUserConfirmationCodes)
            {
                await userConfirmationCodeRepository.DeleteAsync(oldUserConfirmationCode.Id, cancellationToken);
            }

            Commit();

            return Unit.Value;
        }

        public void Dispose() => userRepository.Dispose();
    }
}
