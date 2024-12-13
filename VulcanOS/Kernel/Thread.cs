namespace VulcanOS.Kernel;

public enum ThreadState { Ready, Running, Blocked, Terminated }

public class Thread
{
    public int Id { get; private set; }
    public ThreadState State { get; set; }
    public int ExecutionTime { get; set; }

    public Thread(int id, int executionTime)
    {
        Id = id;
        ExecutionTime = executionTime;
        State = ThreadState.Ready;
    }

    public void Execute()
    {
        if (ExecutionTime > 0)
        {
            ExecutionTime--;
            if (ExecutionTime == 0)
                State = ThreadState.Terminated;
        }
    }
}