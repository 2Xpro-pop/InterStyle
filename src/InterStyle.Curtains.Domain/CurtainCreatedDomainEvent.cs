using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Curtains.Domain;

public sealed record CurtainCreatedDomainEvent(
    CurtainId Id,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
