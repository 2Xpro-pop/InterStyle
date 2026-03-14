using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="UpsertCurtainTranslationCommand"/>.
/// </summary>
public sealed class UpsertCurtainTranslationCommandHandler(
    ICurtainRepository curtainRepository,
    ICurtainCacheInvalidator cacheInvalidator)
    : IRequestHandler<UpsertCurtainTranslationCommand>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;
    private readonly ICurtainCacheInvalidator _cacheInvalidator = cacheInvalidator;

    /// <inheritdoc />
    public async Task Handle(UpsertCurtainTranslationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var curtainId = new CurtainId(request.CurtainId);
        var curtain = await _curtainRepository.GetByIdAsync(curtainId, cancellationToken)
            ?? throw new InvalidOperationException($"Curtain with ID {request.CurtainId} not found.");

        var locale = Locale.Create(request.Locale);
        var name = CurtainName.Create(request.Name);
        var description = Description.Create(request.Description);

        curtain.UpsertTranslation(locale, name, description);

        _curtainRepository.Update(curtain);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateAllCurtainsAsync(cancellationToken);
    }
}
