using Unity.Jobs;

public struct SteeringRandomizer : IJobParallelFor
{
    public void Execute(int index)
    {
        // Here we do the work
        throw new System.NotImplementedException();
    }
    
    public static JobHandle Do()
    {
        // Here we setup the job, schedule it and return the handle
        // ... scheduling the job
    /*    var job = new SquareNumbersJob { Nums = myArray };
        return job.Schedule(
            myArray.Length,    // count
            100);              // batch size
    */
        return new();
    }
}