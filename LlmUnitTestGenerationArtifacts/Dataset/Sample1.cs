namespace Dataset.Sample1;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<DepartmentModel> GetAsync(int id)
    {
        var result = await _departmentRepository.GetAsync(id);
        return MapEntityToModel(result);
    }

    public async Task<ICollection<DepartmentModel>> ListAsync()
    {
        var result = await _departmentRepository.ListAsync();
        return result.Select(d => MapEntityToModel(d)).ToList();
    }

    private static DepartmentModel MapEntityToModel(DepartmentEntity entity)
    {
        return new DepartmentModel()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }
}

public interface IDepartmentService
{
    Task<DepartmentModel> GetAsync(int id);
    Task<ICollection<DepartmentModel>> ListAsync();
}

public interface IDepartmentRepository
{
    Task<DepartmentEntity> GetAsync(int id);
    Task<ICollection<DepartmentEntity>> ListAsync();
}

public class DepartmentModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class DepartmentEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
