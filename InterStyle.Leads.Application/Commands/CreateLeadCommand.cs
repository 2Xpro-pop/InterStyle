using MediatR;
using InterStyle.Leads.Domain;

namespace InterStyle.Leads.Application.Commands;

public sealed record CreateLeadCommand(
    string CustomerName,
    string PhoneNumber,
    int ServiceTypeId,
    LeadRequestDetails RequestDetails,
    int SourceId
) : IRequest<LeadId>;