using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace InterStyle.Leads.Domain;

public sealed class Lead : AggregateRoot<LeadId>
{
    // For EF
    private Lead()
    {
    }

    private Lead(
        LeadId id,
        CustomerName customerName,
        PhoneNumber phoneNumber,
        LeadServiceType serviceType,
        LeadStatus status,
        LeadRequestDetails requestDetails,
        LeadSource leadSource,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
        : base(id)
    {
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        ServiceType = serviceType;
        Status = status;
        RequestDetails = requestDetails;
        Source = leadSource;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public CustomerName CustomerName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public LeadServiceType ServiceType { get; private set; } = LeadServiceType.Consultation;
    public LeadStatus Status { get; private set; } = LeadStatus.New;
    public LeadRequestDetails RequestDetails { get; private set; } = LeadRequestDetails.Empty();
    public LeadSource Source { get; private set; } = LeadSource.Unknown;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }


    public static Lead Create(
        CustomerName customerName,
        PhoneNumber phoneNumber,
        LeadServiceType serviceType,
        LeadRequestDetails requestDetails,
        LeadSource source,
        DateTimeOffset createdAtUtc)
    {
        if (customerName == default)
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        if (phoneNumber == default)
        {
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }

        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(requestDetails);

        var lead = new Lead(
            id: LeadId.New(),
            customerName: customerName,
            phoneNumber: phoneNumber,
            serviceType: serviceType,
            status: LeadStatus.New,
            requestDetails: requestDetails,
            leadSource: source,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: createdAtUtc);

        lead.AddDomainEvent(new LeadCreatedDomainEvent(lead.Id, createdAtUtc));
        return lead;
    }


    /// <summary>
    /// Rehydrates the aggregate from persistence without raising domain events.
    /// Should be used only by repositories.
    /// </summary>
    internal static Lead Rehydrate(
        LeadId id,
        CustomerName customerName,
        PhoneNumber phoneNumber,
        LeadServiceType serviceType,
        LeadStatus status,
        LeadRequestDetails requestDetails,
        LeadSource source,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        if (id == default)
        {
            throw new InvalidOperationException("LeadId cannot be empty during rehydration.");
        }

        if (customerName == default)
        {
            throw new InvalidOperationException("Corrupted data: CustomerName is empty.");
        }

        if (phoneNumber == default)
        {
            throw new InvalidOperationException("Corrupted data: PhoneNumber is empty.");
        }

        return new Lead(
            id: id,
            customerName: customerName,
            phoneNumber: phoneNumber,
            serviceType: serviceType,
            status: status,
            requestDetails: requestDetails,
            leadSource: source,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc);
    }
}