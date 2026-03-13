using InterStyle.Shared;

namespace InterStyle.Curtains.Domain;

public sealed class Curtain : AggregateRoot<CurtainId>
{
    private readonly List<CurtainTranslation> _translations = [];

    // for EF
    private Curtain()
    {
    }

    private Curtain(CurtainId id, PictureUrl pictureUrl, PictureUrl previewUrl)
        : base(id)
    {
        PictureUrl = pictureUrl;
        PreviewUrl = previewUrl;
    }

    public IReadOnlyCollection<CurtainTranslation> Translations => _translations;

    public PictureUrl PictureUrl
    {
        get; private set;
    }

    public PictureUrl PreviewUrl
    {
        get; private set;
    }

    public static Curtain Create(Locale locale, CurtainName name, Description description, PictureUrl pictureUrl, PictureUrl previewUrl)
    {
        var curtain = new Curtain(CurtainId.New(), pictureUrl, previewUrl);

        curtain._translations.Add(CurtainTranslation.Create(locale, name, description));

        curtain.AddDomainEvent(new CurtainCreatedDomainEvent(curtain.Id, DateTimeOffset.UtcNow));

        return curtain;
    }

    public void UpsertTranslation(Locale locale, CurtainName name, Description description)
    {
        var existing = _translations.SingleOrDefault(t => t.Locale == locale);

        if (existing is null)
        {
            _translations.Add(CurtainTranslation.Create(locale, name, description));
            AddDomainEvent(new CurtainTranslationAddedDomainEvent(Id, locale.Value, DateTimeOffset.UtcNow));
            return;
        }

        if (existing.Name == name && existing.Description == description)
        {
            return;
        }

        existing.Update(name, description);
        AddDomainEvent(new CurtainTranslationUpdatedDomainEvent(Id, locale.Value, DateTimeOffset.UtcNow));
    }

    public void RemoveTranslation(Locale locale)
    {
        var existing = _translations.SingleOrDefault(t => t.Locale == locale);

        if (existing is null)
        {
            return;
        }

        if (_translations.Count == 1)
        {
            throw new InvalidOperationException("Cannot remove the last translation.");
        }

        _translations.Remove(existing);
        AddDomainEvent(new CurtainTranslationRemovedDomainEvent(Id, locale.Value, DateTimeOffset.UtcNow));
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

    internal static Curtain Rehydrate(CurtainId id, PictureUrl pictureUrl, PictureUrl previewUrl, IEnumerable<CurtainTranslation> translations)
    {
        var curtain = new Curtain(id, pictureUrl, previewUrl);
        curtain._translations.AddRange(translations);
        return curtain;
    }
}
