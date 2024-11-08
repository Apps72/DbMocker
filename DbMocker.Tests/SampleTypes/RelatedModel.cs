using System;

namespace DbMocker.Tests.SampleTypes
{
    public class RelatedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key
        public Guid NavigationModelId { get; set; }

        // Navigation property
        public NavigationModel NavigationModel { get; set; }
    }
}
