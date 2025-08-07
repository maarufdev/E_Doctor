namespace E_Doctor.Core.Domain.Entities;
public class BaseEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}