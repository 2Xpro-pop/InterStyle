using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="ChangeCurtainPictureCommand"/>.
/// </summary>
public sealed class ChangeCurtainPictureCommandHandler(
    ICurtainRepository curtainRepository,
    ICurtainCacheInvalidator cacheInvalidator)
    : IRequestHandler<ChangeCurtainPictureCommand>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;
    private readonly ICurtainCacheInvalidator _cacheInvalidator = cacheInvalidator;

    /// <inheritdoc />
    public async Task Handle(ChangeCurtainPictureCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var curtainId = new CurtainId(request.CurtainId);
        var curtain = await _curtainRepository.GetByIdAsync(curtainId, cancellationToken)
            ?? throw new InvalidOperationException($"Curtain with ID {request.CurtainId} not found.");

        var newPictureUrl = PictureUrl.Create(request.NewPictureUrl);
        curtain.ChangePictureUrl(newPictureUrl);

        _curtainRepository.Update(curtain);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateAllCurtainsAsync(cancellationToken);
    }
}
