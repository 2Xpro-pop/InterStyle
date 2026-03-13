using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="ChangeCurtainPreviewCommand"/>.
/// </summary>
public sealed class ChangeCurtainPreviewCommandHandler(
    ICurtainRepository curtainRepository,
    ICurtainCacheInvalidator cacheInvalidator)
    : IRequestHandler<ChangeCurtainPreviewCommand>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;
    private readonly ICurtainCacheInvalidator _cacheInvalidator = cacheInvalidator;

    /// <inheritdoc />
    public async Task Handle(ChangeCurtainPreviewCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var curtainId = new CurtainId(request.CurtainId);
        var curtain = await _curtainRepository.GetByIdAsync(curtainId, cancellationToken)
            ?? throw new InvalidOperationException($"Curtain with ID {request.CurtainId} not found.");

        var newPreviewUrl = PictureUrl.Create(request.NewPreviewUrl);
        curtain.ChangePreviewUrl(newPreviewUrl);

        _curtainRepository.Update(curtain);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateAllCurtainsAsync(cancellationToken);
    }
}
