using System.Text;
using APBD1.Data.Models;
using APBD1.Repositories;

namespace APBD1;

public class CommandPalette(UserRepository users, DeviceRepository devices, LeaseRepository leases)
{
    public async Task RunReplAsync(List<string> exitCommands)
    {
        Console.WriteLine("=== Equipment Lease Management System ===");
        Console.WriteLine("Type 'help' to see available commands.");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;

            if (exitCommands.Contains(input, StringComparer.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                await HandleCommand(input);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }
        }

        Console.WriteLine("Goodbye.");
    }
    private async Task HandleCommand(string input)
    {
        var parts = input.Split(' ', 2);
        var cmd = parts.First().ToLower();
        
        switch (cmd)
        {
            case "help":
                RunPrintHelp();
                break;
            case "add-student":
                await RunAddStudent();
                break;
            case "add-employee":
                await RunAddEmployee();
                break;
            case "list-users":
                await RunListUsers();
                break;
            case "add-laptop":
                await RunAddLaptop();
                break;
            case "add-projector":
                await RunAddProjector();
                break;
            case "add-camera":
                await RunAddCamera();
                break;
            case "list-devices":
                await RunListDevices();
                break;
            case "list-available":
                await RunListAvailable();
                break;
            case "lease":
                await RunCreateLease();
                break;
            case "return":
                await RunReturnDevice();
                break;
            case "set-unavailable":
                await RunSetUnavailable();
                break;
            case "user-leases":
                await RunUserLeases();
                break;
            case "overdue":
                await RunOverdueLeases();
                break;
            case "report":
                await RunReport();
                break;
            default:
                Console.WriteLine($"Uknown command: {cmd}. Type 'help' to list commands");
                break;
        }
    }
    
#region commands

    private async Task RunAddStudent()
    {
            
        Console.Write("Name: ");
        var name = Console.ReadLine()!.Trim();
        Console.Write("Surname: ");
        var surname = Console.ReadLine()!.Trim();
        Console.Write("Index number: ");
        var index = Console.ReadLine()!.Trim();
        Console.Write("Year of study: ");
        var year = ushort.Parse(Console.ReadLine()!.Trim());

        var id = await users.AddUserAsync(new CreateStudentParams
        {
            Name = name, Surname = surname, IndexNumber = index, YearOfStudy = year
        });
        Console.WriteLine($"Student added. ID: {id}");
    }

    private async Task RunAddEmployee()
    {
        Console.Write("Name: ");
        var name = Console.ReadLine()!.Trim();
        Console.Write("Surname: ");
        var surname = Console.ReadLine()!.Trim();
        Console.Write("Salary: ");
        var salary = ulong.Parse(Console.ReadLine()!.Trim());

        var id = await users.AddUserAsync(new CreateEmployeeParams
        {
            Name = name, Surname = surname, Salary = salary
        });
        Console.WriteLine($"Employee added. ID: {id}");
    }

    private async Task RunListUsers()
    {
        var sb = new StringBuilder();
        var all = await users.GetAllUsersAsync();
        Console.WriteLine("=== Users ===");
        foreach (var user in all)
        {
            sb.Append($"User: {user.Name} {user.Surname} ({user.Id})\n");
        }
        Console.WriteLine(sb.ToString());
    }

    private async Task RunAddLaptop()
    {
        Console.Write("CPU vendor (Intel/Amd): ");
        var vendor = Enum.Parse<CpuVendor>(Console.ReadLine()!.Trim(), ignoreCase: true);
        Console.Write("Screen ratio X: ");
        var rx = uint.Parse(Console.ReadLine()!.Trim());
        Console.Write("Screen ratio Y: ");
        var ry = uint.Parse(Console.ReadLine()!.Trim());

        var id = await devices.AddDeviceAsync(new CreateLaptopParams
        {
            CpuVendor = vendor, ScreenRatioX = rx, ScreenRatioY = ry
        });
        Console.WriteLine($"Laptop added. ID: {id}");
    }

    private async Task RunAddProjector()
    {
        Console.Write("Max brightness (lumens): ");
        var brightness = ulong.Parse(Console.ReadLine()!.Trim());
        Console.Write("Max screen ratio X: ");
        var rx = uint.Parse(Console.ReadLine()!.Trim());
        Console.Write("Max screen ratio Y: ");
        var ry = uint.Parse(Console.ReadLine()!.Trim());

        var id = await devices.AddDeviceAsync(new CreateProjectorParams
        {
            MaxBrightness = brightness, MaxScreenRatioX = rx, MaxScreenRatioY = ry
        });
        Console.WriteLine($"Projector added. ID: {id}");
    }

    private async Task RunAddCamera()
    {
        Console.Write("Lens model: ");
        var lens = Console.ReadLine()!.Trim();
        Console.Write("Max zoom: ");
        var zoom = uint.Parse(Console.ReadLine()!.Trim());

        var id = await devices.AddDeviceAsync(new CreateCameraParams
        {
            LensModel = lens, MaxZoom = zoom
        });
        Console.WriteLine($"Camera added. ID: {id}");
    }

    private async Task RunListDevices()
    {
        var all = await devices.GetAllDevicesAsync();
        if (all.Count == 0) { Console.WriteLine("No devices registered."); return; }
        foreach (var d in all)
            Console.WriteLine($"  [{d.Id}] {d.Name} | {d.GetType().Name} | {d.Status}" +
                              (d is { Status: AvailabilityStatus.Unavailable, UnavailabilityReason: not null }
                                  ? $" ({d.UnavailabilityReason})" : ""));
    }

    private async Task RunListAvailable()
    {
        var available = await devices.GetAvailableDevicesAsync();
        if (available.Count == 0) { Console.WriteLine("No devices currently available."); return; }
        foreach (var d in available)
            Console.WriteLine($"  [{d.Id}] {d.Name} | {d.GetType().Name}");
    }

    private async Task RunCreateLease()
    {
        Console.Write("User ID: ");
        var userId = Guid.Parse(Console.ReadLine()!.Trim());
        Console.Write("Device ID: ");
        var deviceId = Guid.Parse(Console.ReadLine()!.Trim());
        Console.Write("End date (yyyy-MM-dd): ");
        var endDate = DateTimeOffset.Parse(Console.ReadLine()!.Trim());

        var id = await leases.CreateLeaseAsync(new CreateLeaseParams
        {
            UserId = userId, DeviceId = deviceId, EndDate = endDate
        });
        Console.WriteLine($"Lease created. ID: {id}");
    }

    private async Task RunReturnDevice()
    {
        Console.Write("Lease ID: ");
        var leaseId = Guid.Parse(Console.ReadLine()!.Trim());
        await leases.TerminateLeaseAsync(leaseId);

        var lease = await leases.GetLeaseByIdAsync(leaseId);
        Console.WriteLine(lease?.LateFee is > 0
            ? $"Returned. Late fee: {lease.LateFee:F2}"
            : "Returned on time. No late fee.");
    }

    private async Task RunSetUnavailable()
    {
        Console.Write("Device ID: ");
        var deviceId = Guid.Parse(Console.ReadLine()!.Trim());
        Console.Write("Reason (optional): ");
        var reason = Console.ReadLine()!.Trim();
        await devices.SetUnavailableAsync(deviceId, string.IsNullOrEmpty(reason) ? null : reason);
        Console.WriteLine("Device marked as unavailable.");
    }

    private async Task RunUserLeases()
    {
        Console.Write("User ID: ");
        var userId = Guid.Parse(Console.ReadLine()!.Trim());
        var active = await leases.GetActiveLeasesByUserAsync(userId);
        if (active.Count == 0) { Console.WriteLine("No active leases for this user."); return; }
        foreach (var l in active)
            Console.WriteLine($"  [{l.Id}] Device: {l.Device.Name} | Started: {l.StartDate:yyyy-MM-dd} | Due: {l.EndDate:yyyy-MM-dd}");
    }

    private async Task RunOverdueLeases()
    {
        var overdue = await leases.GetOverdueLeasesAsync();
        if (overdue.Count == 0) { Console.WriteLine("No overdue leases."); return; }
        foreach (var l in overdue)
            Console.WriteLine($"  [{l.Id}] {l.Leaser.Name} {l.Leaser.Surname} | Device: {l.Device.Name} | Due: {l.EndDate:yyyy-MM-dd}");
    }

    private async Task RunReport()
    {
        var allDevices = await devices.GetAllDevicesAsync();
        var available = allDevices.Count(d => d.Status == AvailabilityStatus.Available);
        var leased = allDevices.Count(d => d.Status == AvailabilityStatus.Leased);
        var unavailable = allDevices.Count(d => d.Status == AvailabilityStatus.Unavailable);
        var overdue = await leases.GetOverdueLeasesAsync();
        var allUsers = await users.GetAllUsersAsync();

        Console.WriteLine("=== Summary Report ===");
        Console.WriteLine($"Users:       {allUsers.Count} (Students: {allUsers.OfType<Student>().Count()}, Employees: {allUsers.OfType<Employee>().Count()})");
        Console.WriteLine($"Devices:     {allDevices.Count} total | {available} available | {leased} leased | {unavailable} unavailable");
        Console.WriteLine($"Overdue:     {overdue.Count} lease(s)");
        if (overdue.Count > 0)
            foreach (var l in overdue)
                Console.WriteLine($"             [{l.Id}] {l.Leaser.Name} {l.Leaser.Surname} — {l.Device.Name} (due {l.EndDate:yyyy-MM-dd})");
    }

    private void RunPrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add-student      Add a new student");
        Console.WriteLine("  add-employee     Add a new employee");
        Console.WriteLine("  add-laptop       Register a new laptop");
        Console.WriteLine("  add-projector    Register a new projector");
        Console.WriteLine("  add-camera       Register a new camera");
        Console.WriteLine("  list-devices     List all equipment");
        Console.WriteLine("  list-available   List available equipment");
        Console.WriteLine("  lease            Lease equipment to a user");
        Console.WriteLine("  return           Return leased equipment");
        Console.WriteLine("  set-unavailable  Mark equipment as unavailable");
        Console.WriteLine("  user-leases      List active leases for a user");
        Console.WriteLine("  overdue          List overdue leases");
        Console.WriteLine("  report           Show summary report");
        Console.WriteLine("  exit / :q        Quit");
    }
#endregion
}