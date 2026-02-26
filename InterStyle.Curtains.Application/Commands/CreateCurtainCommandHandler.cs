using InterStyle.Curtains.Domain;
using MediatR;

namespace InterStyle.Curtains.Application.Commands;

/// <summary>
/// Handler for <see cref="CreateCurtainCommand"/>.
/// </summary>
public sealed class CreateCurtainCommandHandler(ICurtainRepository curtainRepository)
    : IRequestHandler<CreateCurtainCommand, CurtainId>
{
    private readonly ICurtainRepository _curtainRepository = curtainRepository;

    /// <inheritdoc />
    public async Task<CurtainId> Handle(CreateCurtainCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = CurtainName.Create(request.Name);
        var description = Description.Create(request.Description);
        var pictureUrl = PictureUrl.Create(request.PictureUrl);
        var previewUrl = PictureUrl.Create(request.PreviewUrl);

        var curtain = Curtain.Create(
            name: name,
            description: description,
            pictureUrl: pictureUrl,
            previewUrl: previewUrl);

        await _curtainRepository.AddAsync(curtain, cancellationToken);
        await _curtainRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return curtain.Id;
    }
}
