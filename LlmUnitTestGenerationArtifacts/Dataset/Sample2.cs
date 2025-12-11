using System.Text.RegularExpressions;

namespace Dataset.Sample2;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeModel> GetAsync(int id)
    {
        var result = await _employeeRepository.GetAsync(id);
        return MapEntityToModel(result);
    }

    public async Task<ICollection<EmployeeModel>> ListAsync()
    {
        var result = await _employeeRepository.ListAsync();
        return result.Select(e => MapEntityToModel(e)).ToList();
    }

    public async Task<ICollection<EmployeeModel>> ListFilteredAsync(
        string name,
        string country,
        string city,
        decimal minSalary,
        decimal maxSalary,
        int departmentId,
        int positionId)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city) &&
            minSalary <= 0 && maxSalary <= 0 && departmentId <= 0 && positionId <= 0)
        {
            return await ListAsync();
        }

        var result = await _employeeRepository.ListFilteredAsync(
            name, country, city, minSalary, maxSalary, departmentId, positionId);
        return result.Select(e => MapEntityToModel(e)).ToList();
    }

    public async Task<int> AddAsync(EmployeeModel employeeModel)
    {
        ValidateEmployee(employeeModel);
        return await _employeeRepository.AddAsync(MapModelToEntity(employeeModel));
    }

    public async Task<int> UpdateAsync(EmployeeModel employeeModel)
    {
        ValidateEmployee(employeeModel);

        if (employeeModel.Id <= 0)
        {
            throw new ArgumentException(nameof(employeeModel.Id));
        }

        return await _employeeRepository.UpdateAsync(MapModelToEntity(employeeModel));
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _employeeRepository.DeleteAsync(id);
    }

    private static void ValidateEmployee(EmployeeModel employeeModel)
    {
        if (employeeModel == null)
        {
            throw new ArgumentNullException(nameof(employeeModel));
        }

        Regex wordPattern = new Regex("^[a-zA-Z- ]+$");
        Regex phonePattern = new Regex("^[+][0-9]{10,12}$");

        if (string.IsNullOrWhiteSpace(employeeModel.FirstName))
        {
            throw new ArgumentException(nameof(employeeModel.FirstName));
        }
        else
        {
            employeeModel.FirstName = employeeModel.FirstName.Trim();
            if (!wordPattern.IsMatch(employeeModel.FirstName) || employeeModel.FirstName.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.FirstName));
            }
        }

        if (string.IsNullOrWhiteSpace(employeeModel.LastName))
        {
            throw new ArgumentException(nameof(employeeModel.LastName));
        }
        else
        {
            employeeModel.LastName = employeeModel.LastName.Trim();
            if (!wordPattern.IsMatch(employeeModel.LastName) || employeeModel.LastName.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.LastName));
            }
        }

        if (string.IsNullOrWhiteSpace(employeeModel.MiddleName))
        {
            throw new ArgumentException(nameof(employeeModel.MiddleName));
        }
        else
        {
            employeeModel.MiddleName = employeeModel.MiddleName.Trim();
            if (!wordPattern.IsMatch(employeeModel.MiddleName) || employeeModel.MiddleName.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.MiddleName));
            }
        }

        if (employeeModel.BirthDate <= new DateTime(1950, 1, 1) || employeeModel.BirthDate >= new DateTime(2010, 1, 1))
        {
            throw new ArgumentException(nameof(employeeModel.BirthDate));
        }

        if (string.IsNullOrWhiteSpace(employeeModel.Country))
        {
            employeeModel.Country = null;
        }
        else
        {
            employeeModel.Country = employeeModel.Country.Trim();
            if (!wordPattern.IsMatch(employeeModel.Country) || employeeModel.Country.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.Country));
            }
        }

        if (string.IsNullOrWhiteSpace(employeeModel.City))
        {
            employeeModel.City = null;
        }
        else
        {
            employeeModel.City = employeeModel.City.Trim();
            if (!wordPattern.IsMatch(employeeModel.City) || employeeModel.City.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.City));
            }
        }

        if (string.IsNullOrWhiteSpace(employeeModel.Address))
        {
            employeeModel.Address = null;
        }
        else
        {
            employeeModel.Address = employeeModel.Address.Trim();
            if (employeeModel.Address.Length > 50)
            {
                throw new ArgumentException(nameof(employeeModel.City));
            }
        }

        if (string.IsNullOrWhiteSpace(employeeModel.Phone))
        {
            employeeModel.Phone = null;
        }
        else
        {
            employeeModel.Phone = employeeModel.Phone.Trim();

            if (!employeeModel.Phone.StartsWith('+'))
            {
                employeeModel.Phone = employeeModel.Phone.Insert(0, "+");
            }

            if (!phonePattern.IsMatch(employeeModel.Phone))
            {
                throw new ArgumentException(nameof(employeeModel.Phone));
            }
        }

        if (employeeModel.HireDate <= employeeModel.BirthDate || employeeModel.HireDate <= new DateTime(1960, 1, 1) || employeeModel.HireDate > DateTime.Now)
        {
            throw new ArgumentException(nameof(employeeModel.HireDate));
        }

        if (employeeModel.Salary <= 0)
        {
            throw new ArgumentException(nameof(employeeModel.Salary));
        }

        if (employeeModel.DepartmentId <= 0)
        {
            throw new ArgumentException(nameof(employeeModel.DepartmentId));
        }

        if (employeeModel.PositionId <= 0)
        {
            throw new ArgumentException(nameof(employeeModel.PositionId));
        }
    }

    private static EmployeeModel MapEntityToModel(EmployeeEntity entity)
    {
        return new EmployeeModel()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            MiddleName = entity.MiddleName,
            BirthDate = entity.BirthDate,
            Country = entity.Country,
            City = entity.City,
            Address = entity.Address,
            Phone = entity.Phone,
            HireDate = entity.HireDate,
            Salary = entity.Salary,
            DepartmentId = entity.DepartmentId,
            Department = entity.Department,
            PositionId = entity.PositionId,
            Position = entity.Position
        };
    }

    private static EmployeeEntity MapModelToEntity(EmployeeModel model)
    {
        return new EmployeeEntity()
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            BirthDate = model.BirthDate,
            Country = model.Country,
            City = model.City,
            Address = model.Address,
            Phone = model.Phone,
            HireDate = model.HireDate,
            Salary = model.Salary,
            DepartmentId = model.DepartmentId,
            Department = model.Department,
            PositionId = model.PositionId,
            Position = model.Position
        };
    }
}

public interface IEmployeeService
{
    Task<EmployeeModel> GetAsync(int id);
    Task<ICollection<EmployeeModel>> ListAsync();
    Task<ICollection<EmployeeModel>> ListFilteredAsync(
        string name,
        string country,
        string city,
        decimal minSalary,
        decimal maxSalary,
        int departmentId,
        int positionId);
    Task<int> AddAsync(EmployeeModel employeeModel);
    Task<int> UpdateAsync(EmployeeModel employeeModel);
    Task<int> DeleteAsync(int id);
}

public interface IEmployeeRepository
{
    Task<EmployeeEntity> GetAsync(int id);
    Task<ICollection<EmployeeEntity>> ListAsync();
    Task<ICollection<EmployeeEntity>> ListFilteredAsync(
        string name,
        string country,
        string city,
        decimal minSalary,
        decimal maxSalary,
        int departmentId,
        int positionId);
    Task<int> AddAsync(EmployeeEntity employeeEntity);
    Task<int> UpdateAsync(EmployeeEntity employeeEntity);
    Task<int> DeleteAsync(int id);
}

public class EmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public string Department { get; set; } = null!;
    public int PositionId { get; set; }
    public string Position { get; set; } = null!;
}

public class EmployeeEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public string Department { get; set; } = null!;
    public int PositionId { get; set; }
    public string Position { get; set; } = null!;
}
