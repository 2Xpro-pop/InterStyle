using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="ChangeCurtainDescriptionCommand"/>.
/// </summary>
public sealed class ChangeCurtainDescriptionCommandHandler(ICurtainRepository curtainRepository)
    : IRequestHandler<ChangeCurtainDescriptionCommand>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;

    /// <inheritdoc />
    public async Task Handle(ChangeCurtainDescriptionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var curtainId = new CurtainId(request.CurtainId);
        var curtain = await _curtainRepository.GetByIdAsync(curtainId, cancellationToken)
            ?? throw new InvalidOperationException($"Curtain with ID {request.CurtainId} not found.");

        var newDescription = Description.Create(request.NewDescription);
        curtain.ChangeDescription(newDescription);

        _curtainRepository.Update(curtain);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
