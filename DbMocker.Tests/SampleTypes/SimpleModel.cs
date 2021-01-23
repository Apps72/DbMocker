using System;

namespace DbMocker.Tests.SampleTypes
{
    public class SimpleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public long? Checksum { get; set; }
        public DateTime TimestampCreated { get; set; }
        public DateTime? TimestampModified { get; set; }

        public static long RandomValue => DateTime.UtcNow.Ticks;

        public static SimpleModel RandomInstance()
        {
            return new SimpleModel()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Value = DateTime.UtcNow.Millisecond,
                TimestampCreated = DateTime.UtcNow
            };
        }

        public SimpleModel CalculateChecksum()
        {
            Checksum = DateTime.UtcNow.Ticks;
            return SetUpdated();
        }

        public SimpleModel SetUpdated()
        {
            TimestampModified = DateTime.UtcNow;
            return this;
        }
    }
}
