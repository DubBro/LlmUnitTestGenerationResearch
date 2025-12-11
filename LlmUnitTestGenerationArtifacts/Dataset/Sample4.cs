using AutoMapper;

namespace Dataset.Sample4;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<CompanyModel> GetAsync(int id = 1)
    {
        var result = await _companyRepository.GetAsync(id);
        return _mapper.Map<CompanyModel>(result);
    }
}

public interface ICompanyService
{
    Task<CompanyModel> GetAsync(int id = 1);
}

public interface ICompanyRepository
{
    Task<CompanyEntity> GetAsync(int id = 1);
}

public class CompanyModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class CompanyEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
