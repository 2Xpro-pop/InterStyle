using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

public sealed class Curtain: AggregateRoot<CurtainId>
{
    // for EF
    private Curtain()
    {

    }

    private Curtain(CurtainId id, CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
        : base(id)
    {
        Name = name;
        Description = description;
        PictureUrl = pictureUrl;
        PreviewUrl = previewUrl;
    }

    public CurtainName Name
    {
        get; private set;
    }

    public Description Description
    {
        get; private set;
    }

    public PictureUrl PictureUrl
    {
        get; private set;
    }

    public PictureUrl PreviewUrl
    {
        get; private set;
    }

    public static Curtain Create(CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
    {
        var curtain = new Curtain(CurtainId.New(), name, description, pictureUrl, previewUrl);

        curtain.AddDomainEvent(new CurtainCreatedDomainEvent(curtain.Id, DateTime.UtcNow));

        return curtain;
    }

    public void ChangeName(CurtainName newName)
    {
        if (Name == newName)
        {
            return;
        }

        var oldName = Name;
        Name = newName;

        AddDomainEvent(new CurtainNameChangedDomainEvent(Id, oldName, newName, DateTimeOffset.UtcNow));
    }

    public void ChangeDescription(Description newDescription)
    {
        if (Description == newDescription)
        {
            return;
        }

        var oldDescription = Description;
        Description = newDescription;

        AddDomainEvent(new CurtainDescriptionChangedDomainEvent(Id, oldDescription, newDescription, DateTimeOffset.UtcNow));
    }

    public void ChangePictureUrl(PictureUrl newPictureUrl)
    {
        if (PictureUrl == newPictureUrl)
        {
            return;
        }

        var oldPictureUrl = PictureUrl;
        PictureUrl = newPictureUrl;

        AddDomainEvent(new CurtainPictureChangedDomainEvent(Id, oldPictureUrl, newPictureUrl, DateTimeOffset.UtcNow));
    }

    public void ChangePreviewUrl(PictureUrl newPreviewUrl)
    {
        if (PreviewUrl == newPreviewUrl)
        {
            return;
        }

        var oldPreviewUrl = PreviewUrl;
        PreviewUrl = newPreviewUrl;

        AddDomainEvent(new CurtainPreviewChangedDomainEvent(Id, oldPreviewUrl, newPreviewUrl, DateTimeOffset.UtcNow));
    }

    internal static Curtain Rehydrate(CurtainId id, CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
    {
        return new Curtain(id, name, description, pictureUrl, previewUrl);
    }
}
