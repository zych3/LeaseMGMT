using System.Text;
using APBD1.Data;
using APBD1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD1.Repositories;

public class LeaseRepository(AppDbContext db, AppOptions opts)
{
    public async Task<Guid> CreateLeaseAsync(CreateLeaseParams param)
    {
        var user = await db.Users.FindAsync(param.UserId) ?? throw new ArgumentException("User not found");
        var device = await db.Devices.FindAsync(param.DeviceId) ?? throw new ArgumentException("Device not found");
        await ValidateLeaseAsync(user, device);

        var lease = new Lease
        {
            EndDate = param.EndDate,
            DeviceId = param.DeviceId,
            Device = device,
            UserId = param.UserId,
            Leaser = user,
        };
        device.Status = AvailabilityStatus.Leased;
        await db.Leases.AddAsync(lease);
        await db.SaveChangesAsync();
        return lease.Id;
    }

    public async Task TerminateLeaseAsync(Guid leaseId)
    {
        var lease = await db.Leases
            .Include(l => l.Device)
            .FirstOrDefaultAsync(l => l.Id == leaseId)
            ?? throw new ArgumentException("Lease not found");

        if (lease.ReturnDate != null)
            throw new ArgumentException("Lease is already terminated");

        lease.ReturnDate = DateTimeOffset.UtcNow;

        if (lease.ReturnDate > lease.EndDate)
        {
            var daysLate = (lease.ReturnDate.Value - lease.EndDate).TotalDays;
            lease.LateFee = (decimal)daysLate * (decimal)opts.LateFeeDaily;
        }

        lease.Device.Status = AvailabilityStatus.Available;
        await db.SaveChangesAsync();
    }

    public async Task<List<Lease>> GetActiveLeasesByUserAsync(Guid userId)
    {
        return await db.Leases
            .Include(l => l.Device)
            .Where(l => l.UserId == userId && l.ReturnDate == null)
            .ToListAsync();
    }

    public async Task<List<Lease>> GetOverdueLeasesAsync()
    {
        var active = await db.Leases
            .Include(l => l.Leaser)
            .Include(l => l.Device)
            .Where(l => l.ReturnDate == null)
            .ToListAsync();
        return active.Where(l => l.EndDate < DateTimeOffset.UtcNow).ToList();
    }

    public async Task<Lease?> GetLeaseByIdAsync(Guid leaseId)
    {
        return await db.Leases
            .Include(l => l.Leaser)
            .Include(l => l.Device)
            .FirstOrDefaultAsync(l => l.Id == leaseId);
    }

    private async Task ValidateLeaseAsync(User user, Device device)
    {
        var activeLeaseCount = await db.Leases
            .CountAsync(l => l.UserId == user.Id && l.ReturnDate == null);

        var maxLeases = user is Student
            ? opts.Constraints.MaxStudentLeases
            : opts.Constraints.MaxEmployeeLeases;

        if (activeLeaseCount >= maxLeases)
        {
            throw new ArgumentException($"This user has reached the maximum number of active leases ({maxLeases})");
        }

        if (device.Status != AvailabilityStatus.Available)
        {
            var sb = new StringBuilder("This device is unavailable");
            if (device.Status == AvailabilityStatus.Unavailable && !string.IsNullOrEmpty(device.UnavailabilityReason))
            {
                sb.Append($"\nUnavailability reason: {device.UnavailabilityReason}");
            }
            throw new ArgumentException(sb.ToString());
        }
    }
}

public class CreateLeaseParams
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset EndDate { get; set; }
}
