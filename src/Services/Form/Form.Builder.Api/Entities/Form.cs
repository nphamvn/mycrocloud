﻿using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Entities
{
    public class Form
    {
        [Key]
        public int Id { get; set; }
        
        public string? Description { get; set; }
        
        public required string UserId { get; set; }
        
        public required string Name { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public ICollection<FormField> Fields { get; set; } = [];
    }
}
