using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data.Repositories;

public class QualityAndAssuranceRepository(IDbContextFactory<DozdayDbContext> contextFactory) : IQualityAndAssuranceRepository
{
    private readonly IDbContextFactory<DozdayDbContext> _contextFactory = contextFactory;

    public async Task AddAsync(QualityAndAssurance qualityAssurance)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.QualityAndAssurances.AddAsync(qualityAssurance);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.QualityAndAssurances.Where(q => q.Id == id).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<QualityAndAssurance>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QualityAndAssurances.ToListAsync();
    }

    public async Task<QualityAndAssurance?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QualityAndAssurances.Where(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<QualityAndAssurance>> SearchAsync(string text)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QualityAndAssurances.Where(q => q.Tittle.ToLower().Contains(text.ToLower()) || q.AssuranceMarkdown.ToLower().Contains(text.ToLower()) || q.Id.ToString().Contains(text)).ToListAsync();
    }

    public async Task UpdateAsync(QualityAndAssurance qualityAssurance)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.QualityAndAssurances.Where(q => q.Id == qualityAssurance.Id).ExecuteUpdateAsync(q => q
            .SetProperty(q => q.Tittle, qualityAssurance.Tittle)
            .SetProperty(q => q.AssuranceMarkdown, qualityAssurance.AssuranceMarkdown));
    }
}