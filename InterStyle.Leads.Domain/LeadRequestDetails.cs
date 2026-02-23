using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Details needed for estimation/consultation.
/// </summary>
public sealed record LeadRequestDetails(
    string? Area,
    string? Address,
    int? WindowsCount,
    string? Notes,
    bool RequiresInstallation)
{
    public static LeadRequestDetails Empty() =>
        new(
            Area: null,
            Address: null,
            WindowsCount: null,
            Notes: null,
            RequiresInstallation: false);
}