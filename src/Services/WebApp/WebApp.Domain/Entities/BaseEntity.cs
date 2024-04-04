using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain.Entities;
public abstract class BaseEntity {
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    [ConcurrencyCheck]
    public Guid Version { get; set; }
}