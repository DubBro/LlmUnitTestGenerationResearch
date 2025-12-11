using System.Text.RegularExpressions;

namespace Dataset.Sample7;

public class WalkService : IWalkService
{
    private readonly IWalkRepository _walkRepository;

    public WalkService(IWalkRepository walkRepository)
    {
        _walkRepository = walkRepository;
    }

    public async Task<IEnumerable<WalkDTO>> Get10LongestWalksAsync(string imei)
    {
        ValidateImei(imei);

        var data = await _walkRepository.Get10LongestWalksAsync(imei);

        var result = new List<WalkDTO>();
        int i = 1;
        foreach (var item in data)
        {
            result.Add(new WalkDTO
            {
                Number = i,
                Distance = item.Distance,
                Duration = item.Duration,
            });

            i++;
        }

        return result;
    }

    public async Task<WalksGeneralInfoDTO> GetWalksGeneralInfoAsync(string imei)
    {
        ValidateImei(imei);

        var data = await _walkRepository.GetWalksGeneralInfoAsync(imei);

        return new WalksGeneralInfoDTO()
        {
            TotalCount = data.TotalCount,
            TotalDistance = data.TotalDistance,
            TotalDuration = data.TotalDuration,
        };
    }

    private void ValidateImei(string imei)
    {
        if (!Regex.IsMatch(imei, @"^\d{15}$"))
        {
            throw new BusinessException($"Invalid IMEI {imei}.");
        }
    }
}

public interface IWalkService
{
    Task<IEnumerable<WalkDTO>> Get10LongestWalksAsync(string imei);
    Task<WalksGeneralInfoDTO> GetWalksGeneralInfoAsync(string imei);
}

public interface IWalkRepository
{
    Task<IEnumerable<WalkEntity>> Get10LongestWalksAsync(string imei);
    Task<WalksGeneralInfoEntity> GetWalksGeneralInfoAsync(string imei);
}

public class WalkDTO
{
    public int Number { get; set; }
    public decimal Distance { get; set; }
    public decimal Duration { get; set; }
}

public class WalksGeneralInfoDTO
{
    public int TotalCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalDuration { get; set; }
}

public class WalkEntity
{
    public decimal Distance { get; set; }
    public decimal Duration { get; set; }
}

public class WalksGeneralInfoEntity
{
    public int TotalCount { get; set; }
    public decimal TotalDistance { get; set; }
    public decimal TotalDuration { get; set; }
}

public class BusinessException : Exception
{
    public BusinessException()
    {
    }

    public BusinessException(string message)
        : base(message)
    {
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
