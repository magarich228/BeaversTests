using System.Collections.Concurrent;
using BeaversTests.TestRunnerController.Core;

namespace BeaversTests.TestRunnerController.App;

public class AgentsContext
{
    public ConcurrentDictionary<Guid, TestAgent> TestAgents { get; } = new();
}