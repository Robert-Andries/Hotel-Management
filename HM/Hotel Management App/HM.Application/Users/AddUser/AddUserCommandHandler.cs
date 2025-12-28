using HM.Application.Abstractions.Messaging;
using HM.Application.Users.Services;
using HM.Domain.Abstractions;
using HM.Domain.Users;
using HM.Domain.Users.Abstractions;

namespace HM.Application.Users.AddUser;

internal sealed class AddUserCommandHandler : ICommandHandler<AddUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserCreationService _userCreationService;
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        UserCreationService userCreationService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userCreationService = userCreationService;
    }

    public async Task<Result<Guid>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var userResult = _userCreationService.CreateUser(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.CountryCode,
            request.DateOfBirth);

        if (userResult.IsFailure) return Result.Failure<Guid>(userResult.Error);
        var user = userResult.Value;

        if (!await _userRepository.IsEmailUniqueAsync(user.Contact.Email, cancellationToken))
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}