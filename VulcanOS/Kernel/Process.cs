namespace VulcanOS.Kernel;

public enum ProcessState
{
    Ready,
    Running,
    Blocked,
    Terminated
}

public class Process(int id, int priority, int executionTime)
{
    public int Id { get; private set; } = id;
    public int Priority { get; set; } = priority;
    public ProcessState State { get; set; } = ProcessState.Ready;
    public int ExecutionTime { get; set; } = executionTime;
}