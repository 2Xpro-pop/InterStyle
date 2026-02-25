using InterStyle.Leads.Domain;
using InterStyle.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Infrastructure.Persistence;

public sealed class LeadRepository(LeadsDbContext dbContext) : ILeadRepository
{
    private readonly LeadsDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<Lead?> GetByIdAsync(LeadId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Lead>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Lead>().AddAsync(lead, cancellationToken);
    }

    public void Update(Lead lead)
    {
        _dbContext.Set<Lead>().Update(lead);
    }
}