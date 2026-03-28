using APBD1.Data;
using APBD1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD1.Repositories;

public class DeviceRepository(AppDbContext db)
{
    public async Task<Guid> AddDeviceAsync(CreateDeviceParams deviceParams)
    {
        return deviceParams switch
        {
            CreateLaptopParams laptop => await AddLaptopAsync(laptop),
            CreateProjectorParams projector => await AddProjectorAsync(projector),
            CreateCameraParams camera => await AddCameraAsync(camera),
            _ => throw new ArgumentOutOfRangeException(nameof(deviceParams))
        };
    }

#region abstract handling
    private async Task<Guid> AddLaptopAsync(CreateLaptopParams param)
    {
        var laptop = new Laptop
        {
            CpuVendor = param.CpuVendor,
            ScreenRatio = new ScreenRatio { X = param.ScreenRatioX, Y = param.ScreenRatioY }
        };
        await db.Devices.AddAsync(laptop);
        await db.SaveChangesAsync();
        return laptop.Id;
    }

    private async Task<Guid> AddProjectorAsync(CreateProjectorParams param)
    {
        var projector = new Projector
        {
            MaxBrightness = param.MaxBrightness,
            MaxScreenRatio = new ScreenRatio { X = param.MaxScreenRatioX, Y = param.MaxScreenRatioY }
        };
        await db.Devices.AddAsync(projector);
        await db.SaveChangesAsync();
        return projector.Id;
    }

    private async Task<Guid> AddCameraAsync(CreateCameraParams param)
    {
        var camera = new Camera
        {
            LensModel = param.LensModel,
            MaxZoom = param.MaxZoom
        };
        await db.Devices.AddAsync(camera);
        await db.SaveChangesAsync();
        return camera.Id;
    }
#endregion

    public async Task<List<Device>> GetAllDevicesAsync()
    {
        return await db.Devices.ToListAsync();
    }

    public async Task<List<Device>> GetAvailableDevicesAsync()
    {
        return await db.Devices
            .Where(d => d.Status == AvailabilityStatus.Available)
            .ToListAsync();
    }

    public async Task<Device?> GetDeviceByIdAsync(Guid id)
    {
        return await db.Devices.FindAsync(id);
    }

    public async Task SetUnavailableAsync(Guid deviceId, string? reason = null)
    {
        var device = await db.Devices.FindAsync(deviceId)
            ?? throw new ArgumentException("Device not found");

        if (device.Status == AvailabilityStatus.Leased)
            throw new ArgumentException("Cannot mark a currently leased device as unavailable");

        device.Status = AvailabilityStatus.Unavailable;
        device.UnavailabilityReason = reason;
        await db.SaveChangesAsync();
    }
}

#region params
public abstract class CreateDeviceParams;

public class CreateLaptopParams : CreateDeviceParams
{
    public CpuVendor CpuVendor { get; set; }
    public uint ScreenRatioX { get; set; }
    public uint ScreenRatioY { get; set; }
}

public class CreateProjectorParams : CreateDeviceParams
{
    public ulong MaxBrightness { get; set; }
    public uint MaxScreenRatioX { get; set; }
    public uint MaxScreenRatioY { get; set; }
}

public class CreateCameraParams : CreateDeviceParams
{
    public string LensModel { get; set; } = string.Empty;
    public uint MaxZoom { get; set; }
}
#endregion
