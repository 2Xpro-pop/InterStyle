using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Marketing source (for analytics).
/// </summary>
public sealed class LeadSource : Enumeration
{
    public static readonly LeadSource Unknown = new(0, nameof(Unknown));
    public static readonly LeadSource Instagram = new(1, nameof(Instagram));
    public static readonly LeadSource TikTok = new(2, nameof(TikTok));
    public static readonly LeadSource Google = new(3, nameof(Google));
    public static readonly LeadSource Referral = new(4, nameof(Referral));
    public static readonly LeadSource Other = new(5, nameof(Other));

    private LeadSource(int id, string name) : base(id, name)
    {
    }
}