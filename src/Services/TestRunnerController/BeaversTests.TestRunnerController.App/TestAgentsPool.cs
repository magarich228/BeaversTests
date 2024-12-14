using BeaversTests.Common.Application;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestRunnerController.App;

public class TestAgentsPool(
    IUserService userService,
    ITestRunnerControllerContext db) : ITestAgentsPool
{
    public async Task<Guid?> GetAsync(CancellationToken cancellationToken = default)
    {
        var userId = userService.GetCurrentUserId();
        
        var preparedTestAgent = await db.TestAgents
            .Where(a => a.OwnerId.Equals(userId))
            .FirstOrDefaultAsync(
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