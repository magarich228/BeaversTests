using System.Collections.Concurrent;
using BeaversTests.TestRunnerAgent.Core.Tasks;

namespace BeaversTests.TestRunnerAgent.App;

public class TasksContainer
{
    private readonly ConcurrentQueue<ITask> _tasks = new();
    
    public void Register(ITask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        if (task.Type == TaskType.Unknown)
            throw new ArgumentException("Unknown task type.");
        
        _tasks.Enqueue(task);
    }
    
    public bool TryGetNextTask(out ITask? task)
    {
        var dequeue = _tasks.TryDequeue(out task);
        
        dequeue = dequeue && task is not null;
        
        return dequeue;
    }
}