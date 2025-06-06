using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateTests;

public static class StargateTestDb
{
    public static StargateContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new StargateContext(options);
    }
}
