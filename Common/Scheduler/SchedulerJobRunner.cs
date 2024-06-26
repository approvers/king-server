﻿namespace Approvers.King.Common;

public abstract class SchedulerJobRunner
{
    public Predicate<DateTime>? Predicate { get; init; }
    public abstract void Run();
}

public class SchedulerJobRunner<T> : SchedulerJobRunner where T : SchedulerJobPresenterBase, new()
{
    public override void Run()
    {
        new T().RunAsync().Run();
    }
}
