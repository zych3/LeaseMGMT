using System.ComponentModel.DataAnnotations;

namespace APBD1.Data.Models;

public abstract class User 
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
    [MaxLength(100)]
    public string Surname { get; init; } = string.Empty;
    public virtual UserType UserType { get; set; }
    public ICollection<Lease> Leases { get; set; } = [];
}

public enum UserType
{
    Student,
    Employee
}

public class Student : User
{
    public override UserType UserType => UserType.Student;
    [MaxLength(12)]
    public string IndexNumber { get; set; } = string.Empty;
    public ushort YearOfStudy { get; set; } = 1;
}

public class Employee : User
{
    public override UserType UserType => UserType.Employee;
    public ulong Salary { get; set; }
}