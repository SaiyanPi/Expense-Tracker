// using ExpenseTracker.Application.Common.Exceptions;
// using ExpenseTracker.Application.Common.Interfaces.Services;
// using MediatR;

// namespace ExpenseTracker.Application.Features.Identity.Commands.UploadProfileImage;

// public class UploadProfileImageCommandHandler: IRequestHandler<UploadProfileImageCommand, Unit>
// {
//     private readonly IIdentityService _identityService;
//     private readonly IUserAccessor _userAccessor;

//     public UploadProfileImageCommandHandler(
//         IIdentityService identityService,
//         IUserAccessor userAccessor)
//     {
//         _identityService = identityService;
//         _userAccessor = userAccessor;
//     }

//     public async Task<Unit> Handle(
//         UploadProfileImageCommand request,
//         CancellationToken cancellationToken)
//     {
//         if (request.Image is null || request.Image.Length == 0)
//             throw new BadRequestException("An image is required.");

//         var userId = _userAccessor.UserId;
        
//         await _identityService.UploadProfileImageAsync(
//             userId,
//             request.Image,
//             request.FileName,
//             cancellationToken);

//         return Unit.Value;
//     }

// }