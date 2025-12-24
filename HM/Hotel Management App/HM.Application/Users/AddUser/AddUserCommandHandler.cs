using System.Net.Mail;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Users;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;

namespace HM.Application.Users.AddUser;

public sealed class AddUserCommandHandler : ICommandHandler<AddUserCommand, Result>
{
    private readonly ITime _time;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ITime time)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _time = time;
    }

    public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var emailResult = StringToEmail(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);
        var email = emailResult.Value;

        if (await _userRepository.IsEmailUniqueAsync(email, cancellationToken) == false)
        {
            return Result.Failure(UserErrors.EmailNotUnique);
        }

        var name = new Name(request.FirstName, request.LastName);

        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber, request.CountryCode);
        if (phoneNumberResult.IsFailure) return Result.Failure(phoneNumberResult.Error);

        var phoneNumber = phoneNumberResult.Value;

        var contactInfo = new ContactInfo(email, phoneNumber);

        var userResult = User.Create(name, contactInfo, request.DateOfBirth, DateOnly.FromDateTime(_time.NowUtc));
        if (userResult.IsFailure) return Result.Failure(userResult.Error);

        var user = userResult.Value;

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private Result<Email> StringToEmail(string email)
    {
        try
        {
            var emailAddress = new MailAddress(email);
            var output = new Email(emailAddress.User, emailAddress.Host);
            return Result.Success(output);
        }
        catch
        {
            return Result.Failure<Email>(UserErrors.InvalidEmail);
        }
    }
}