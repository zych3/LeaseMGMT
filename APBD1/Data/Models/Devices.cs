using System.ComponentModel.DataAnnotations;

namespace APBD1.Data.Models;

public abstract class Device
{
    protected Device()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Name = BuildName();
    }
    public Guid Id { get; init; } =  Guid.NewGuid();
    [MaxLength(100)]
    public string Name { get; set; } 
    public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
    [MaxLength(500)]
    public string? UnavailabilityReason { get; set; } = null;
    public ICollection<Lease> Leases { get; set; } = [];
    protected abstract string BuildName();
}

public enum AvailabilityStatus
{
    Available,
    Leased,
    Unavailable
}

public class ScreenRatio
{
    public uint X { get; set; }
    public uint Y { get; set; }
}

public class Laptop : Device
{
    public ScreenRatio ScreenRatio { get; set; } = new();
    public CpuVendor CpuVendor { get; set; }

    protected override string BuildName() => $"PJA-LAP-{Id.ToString("N")[..8].ToUpper()}";
}

public enum CpuVendor
{
    Intel,
    Amd
}

public enum CpuArchitecture
{
    X86,
    X64,
    Arm
}

public class Projector : Device
{
    public ScreenRatio MaxScreenRatio { get; set; } = new();
    public ulong MaxBrightness { get; set; }
    protected override string BuildName() => $"PJA-PROJ-{Id.ToString("N")[..8].ToUpper()}";
}

public class Camera : Device
{
    [MaxLength(100)]
    public string LensModel { get; set; } = string.Empty;
    public uint MaxZoom { get; set; }
    protected override string BuildName() => $"PJA-CAM-{Id.ToString("N")[..8].ToUpper()}";
}