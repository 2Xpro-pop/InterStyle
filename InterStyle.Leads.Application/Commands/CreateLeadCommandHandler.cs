using InterStyle.Leads.Domain;
using InterStyle.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Application.Commands;

public sealed class CreateLeadCommandHandler(ILeadRepository leadRepository) : IRequestHandler<CreateLeadCommand, LeadId>
{
    private readonly ILeadRepository _leadRepository = leadRepository;

    public async Task<LeadId> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        var customerName = CustomerName.Create(request.CustomerName);
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

        var serviceType = Enumeration.FromId<LeadServiceType>(request.ServiceTypeId);
        var source = Enumeration.FromId<LeadSource>(request.SourceId);

        var lead = Lead.Create(
            customerName: customerName,
            phoneNumber: phoneNumber,
            serviceType: serviceType,
            requestDetails: request.RequestDetails,
            source: source,
            createdAtUtc: DateTimeOffset.UtcNow);

        await _leadRepository.AddAsync(lead, cancellationToken);
        await _leadRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return lead.Id;
    }
}