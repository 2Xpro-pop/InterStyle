using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

public interface ILeadRepository: IRepository<Lead>
{
    public Task<Lead?> GetByIdAsync(LeadId id, CancellationToken cancellationToken = default);

    public Task AddAsync(Lead lead, CancellationToken cancellationToken = default);

    public void Update(Lead lead);
}
