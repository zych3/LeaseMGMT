namespace APBD1.Data.Models;

public class Lease
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public decimal? LateFee { get; set; }


    #region navigation
    public Guid UserId { get; set; }
    public User Leaser { get; set; }
    public Guid DeviceId { get; set; }
    public Device Device { get; set; }
    #endregion
}