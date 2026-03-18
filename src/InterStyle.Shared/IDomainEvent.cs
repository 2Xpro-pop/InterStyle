using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent: INotification
{
    DateTimeOffset OccurredAtUtc { get; }
}