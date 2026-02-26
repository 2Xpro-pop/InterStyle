using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="ChangeCurtainNameCommand"/>.
/// </summary>
public sealed class ChangeCurtainNameCommandHandler(ICurtainRepository curtainRepository)
    : IRequestHandler<ChangeCurtainNameCommand>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;

    /// <inheritdoc />
    public async Task Handle(ChangeCurtainNameCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var curtainId = new CurtainId(request.CurtainId);
        var curtain = await _curtainRepository.GetByIdAsync(curtainId, cancellationToken)
            ?? throw new InvalidOperationException($"Curtain with ID {request.CurtainId} not found.");

        var newName = CurtainName.Create(request.NewName);
        curtain.ChangeName(newName);

        _curtainRepository.Update(curtain);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
