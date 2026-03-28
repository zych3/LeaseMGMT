namespace APBD1;

public class AppOptions
{
    public Constraints Constraints { get; set; }
    public float LateFeeDaily { get; set; }
    public List<string> ExitCommands { get; set; } = [];
}

public class Constraints
{
    public int MaxStudentLeases { get; set; }
    public int MaxEmployeeLeases { get; set; }
    public ulong MinEmployeeSalary { get; set; }
}