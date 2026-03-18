using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Requested service type.
/// </summary>
public sealed class LeadServiceType : Enumeration
{
    public static readonly LeadServiceType Consultation = new(0, nameof(Consultation));
    public static readonly LeadServiceType Measurement = new(1, nameof(Measurement));
    public static readonly LeadServiceType Sewing = new(2, nameof(Sewing));
    public static readonly LeadServiceType Installation = new(3, nameof(Installation));

    private LeadServiceType(int id, string name) : base(id, name)
    {
    }
}