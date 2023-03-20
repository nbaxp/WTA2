using Microsoft.EntityFrameworkCore;

namespace WTA.Application.Abstractions.Data;

public interface IDbSeed
{
    void Seed(DbContext dbContext);
}
