using System;

namespace Unity.PerformanceTesting
{
    public class PerformanceProjectVersion
    {
        public string ProjectName;
        public string Branch;
        public string Changeset;
        public string Date;

        public void Validate()
        {
            if (string.IsNullOrEmpty(Changeset) || string.IsNullOrEmpty(Date) ||
                string.IsNullOrEmpty(Branch))
            {
                throw new Exception(
                    $"Git version is null or empty. Name: {ProjectName} Changeset: {Changeset} VersionDate: {Date} Branch: {Branch}");
            }
        }
    }
}
