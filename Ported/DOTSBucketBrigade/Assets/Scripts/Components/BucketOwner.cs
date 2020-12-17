
using Unity.Entities;
using Unity.Mathematics;

public struct BucketOwner : IComponentData
{
    // A cohort is a line of firefighters + their fetcher
    // A Value zero means no cohort is associated with this bucket
    //    and it is free to be picked up by a fetcher
    // To find out which cohort index this bucket refers to, take the absolute value
    //    and subtract one.
    // Positive Values: the bucket belongs to the line of bots within the cohort
    // Negative values: the bucket belongs to the fetcher within the cohort
    public int Value;

    int AsCohortIndex()
    {
        return math.abs(Value) - 1;
    }

    public bool IsAssigned()
    {
        return Value != 0;
    }

    bool BelongsToFetcher()
    {
        if (Value < 0)
        {
            return true;
        }
        return false;
    }

    bool BelongsToLine()
    {
        if (Value > 0)
        {
            return true;
        }
        return false;
    }

    public void SetBucketOwner(int cohortIndex, bool isFetcher)
    {
        Value = (cohortIndex + 1) * (isFetcher ? -1 : 1);
    }

    static public BucketOwner CreateBucketOwner(int cohortIndex)
    {
        BucketOwner owner = new BucketOwner();
        owner.SetBucketOwner(cohortIndex, false);
        return owner;
    }
}
