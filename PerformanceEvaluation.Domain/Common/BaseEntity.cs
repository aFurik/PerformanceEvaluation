namespace PerformanceEvaluation.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
