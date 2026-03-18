namespace InterStyle.Curtains.Domain;

/// <summary>
/// Represents a localized name and description for a <see cref="Curtain"/>.
/// Owned by the Curtain aggregate — has no independent identity.
/// </summary>
public sealed class CurtainTranslation
{
    // for EF
    private CurtainTranslation()
    {
    }

    private CurtainTranslation(Locale locale, CurtainName name, Description description)
    {
        Locale = locale;
        Name = name;
        Description = description;
    }

    public Locale Locale
    {
        get; private set;
    }

    public CurtainName Name
    {
        get; private set;
    }

    public Description Description
    {
        get; private set;
    }

    internal static CurtainTranslation Create(Locale locale, CurtainName name, Description description)
    {
        return new CurtainTranslation(locale, name, description);
    }

    internal void Update(CurtainName name, Description description)
    {
        Name = name;
        Description = description;
    }

    internal static CurtainTranslation Rehydrate(Locale locale, CurtainName name, Description description)
    {
        return new CurtainTranslation(locale, name, description);
    }
}
