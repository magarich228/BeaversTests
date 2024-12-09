using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestRunnerController.App;

public class TestAgentsPool(
    ITestRunnerControllerContext db) : ITestAgentsPool
{
    public async Task<Guid?> GetAsync(CancellationToken cancellationToken = default)
    {
        var preparedTestAgent = await db.TestAgents.FirstOrDefaultAsync(
            a => a.Status.Equals(TestAgentStatus.Prepared), 
            cancellationToken);

        if (preparedTestAgent != null)
        {
            return preparedTestAgent.Id;
        }

        var runningTestAgent = await db.TestAgents.FirstOrDefaultAsync(
            a => a.Status.Equals(TestAgentStatus.Running), 
            cancellationToken);

        return runningTestAgent?.Id;
    }
}