using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Lead lifecycle status.
/// </summary>
public sealed class LeadStatus : Enumeration
{
    public static readonly LeadStatus New = new(0, nameof(New));
    public static readonly LeadStatus InProgress = new(1, nameof(InProgress));
    public static readonly LeadStatus Won = new(2, nameof(Won));
    public static readonly LeadStatus Lost = new(3, nameof(Lost));

    private LeadStatus(int id, string name) : base(id, name)
    {
    }
}