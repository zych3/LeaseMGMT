using APBD1.Data;
using APBD1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD1.Repositories;

public class UserRepository(AppDbContext db, AppOptions opts)
{
    public async Task<Guid> AddUserAsync(CreateUserParams userParams)
    {
        if (string.IsNullOrEmpty(userParams.Name))
        {
            throw new ArgumentException(message: "Name cannot be null or empty", paramName: nameof(userParams));
        }

        if (string.IsNullOrEmpty(userParams.Surname))
        {
            throw new ArgumentException(message: "Surname cannot be null or empty", paramName: nameof(userParams));
        }
        
        return userParams switch
        {
            CreateStudentParams student => await AddStudentAsync(student),
            CreateEmployeeParams emp => await AddEmployeeAsync(emp),
            _ => throw new ArgumentOutOfRangeException(nameof(userParams))
        };
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await db.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await db.Users.ToListAsync();
    }

#region abstract handling
    private async Task<Guid> AddStudentAsync(CreateStudentParams param)
    {
        if (string.IsNullOrEmpty(param.IndexNumber))
        {
            throw new ArgumentException("Index Number cannot be null or empty");
        }

        if (param.YearOfStudy < 1)
        {
            throw new ArgumentException("Year of Study cannot be less than 1");
        }
        var student = new Student
        {
            Name = param.Name,
            Surname = param.Surname,
            IndexNumber = param.IndexNumber,
            YearOfStudy = param.YearOfStudy
        };
        await db.Users.AddAsync(student);
        await db.SaveChangesAsync();
        return student.Id;
    }

    private async Task<Guid> AddEmployeeAsync(CreateEmployeeParams param)
    {
        if (param.Salary < opts.Constraints.MinEmployeeSalary)
        {
            throw new ArgumentException($"Salary must be at least {opts.Constraints.MinEmployeeSalary}");
        }
        var employee = new Employee
        {
            Name = param.Name,
            Surname = param.Surname,
            Salary = param.Salary
        };
        await db.Users.AddAsync(employee);
        await db.SaveChangesAsync();
        return employee.Id;
    }
#endregion
}

#region params
public abstract class CreateUserParams
{
    public string Name { get; set; }
    public string Surname { get; set; }
}

public sealed class CreateStudentParams : CreateUserParams
{
    public string IndexNumber { get; set; }
    public ushort YearOfStudy { get; set; }
}

public sealed class CreateEmployeeParams : CreateUserParams
{
    public ulong Salary { get; set; }
}
#endregion
