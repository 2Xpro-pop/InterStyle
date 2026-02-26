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
        get;
    }

    public Description Description
    {
        get; 
    }

    public PictureUrl PictureUrl
    {
        get;
    }

    public PictureUrl PreviewUrl
    {
        get;
    }

    public static Curtain Create(CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
    {
        var curtain = new Curtain(CurtainId.New(), name, description, pictureUrl, previewUrl);

        curtain.AddDomainEvent(new CurtainCreatedDomainEvent(curtain.Id, DateTime.UtcNow));

        return curtain;
    }

    internal static Curtain Rehydrate(CurtainId id, CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
    {
        return new Curtain(id, name, description, pictureUrl, previewUrl);
    }
}
