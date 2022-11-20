using System;
namespace MockServer.Core.Entities
{
    public class FixedResponseHeader : BaseEntity
    {
        public int Id { get; set; }
        public int FixedResponseId { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public int Order { get; set; }
    }
}

