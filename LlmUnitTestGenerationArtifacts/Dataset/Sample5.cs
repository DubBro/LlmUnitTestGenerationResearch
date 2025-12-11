namespace Dataset.Sample5;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;

    public FolderService(IFolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public async Task<FolderDTO> GetFolderAsync(string? path)
    {
        if (path == null)
        {
            var rootFolders = await GetRootFoldersAsync();

            var result = new FolderDTO()
            {
                Id = 0,
                Name = "Root",
                SubFolders = rootFolders,
                ParentId = null,
            };

            return result;
        }

        var folderNames = path.Trim(new[] { ' ', '/' }).Split("/");

        var folder = await _folderRepository.GetFolderByNameAsync(folderNames[0]);

        if (folder == null)
        {
            throw new FolderNotFoundException($"Invalid Request. Folder `{folderNames[0]}` does not exist.");
        }

        for (int i = 1; i < folderNames.Length; i++)
        {
            bool checksum = false;

            foreach (var subfolder in folder!.SubFolders)
            {
                if (folderNames[i] == subfolder.Name)
                {
                    folder = await _folderRepository.GetFolderByNameAsync(folderNames[i], folder.Id);
                    checksum = true;
                    break;
                }
            }

            if (!checksum)
            {
                throw new FolderNotFoundException($"Invalid Request. Folder `{folderNames[i]}` does not exist.");
            }
        }

        var subfolders = new List<FolderDTO>();

        foreach (var subfolder in folder.SubFolders)
        {
            subfolders.Add(new FolderDTO
            {
                Id = subfolder.Id,
                Name = subfolder.Name,
                ParentId = subfolder.ParentId,
            });
        }

        return new FolderDTO
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentId = folder.ParentId,
            SubFolders = subfolders,
        };
    }

    public async Task<IEnumerable<FolderDTO>> GetRootFoldersAsync()
    {
        var data = await _folderRepository.GetRootFoldersAsync();

        var result = new List<FolderDTO>();

        foreach (var folder in data)
        {
            result.Add(new FolderDTO
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentId = folder.ParentId,
                SubFolders = null!,
            });
        }

        return result;
    }

    public async Task<int> AddFolderAsync(FolderDTO folderDto)
    {
        await ValidateFolderDto(folderDto);

        FolderEntity folderEntity = new ()
        {
            Name = folderDto.Name,
            ParentId = folderDto.ParentId,
        };

        var result = await _folderRepository.AddFolderAsync(folderEntity);

        return result;
    }

    private async Task ValidateFolderDto(FolderDTO folderDto)
    {
        if (folderDto == null)
        {
            throw new ArgumentNullException(nameof(folderDto));
        }

        if (string.IsNullOrEmpty(folderDto.Name) || folderDto.Name.Contains("/") || folderDto.Name.Length > 255)
        {
            throw new ArgumentException(nameof(folderDto.Name));
        }

        if (folderDto.ParentId <= 0)
        {
            throw new ArgumentException(nameof(folderDto.ParentId));
        }

        if (await _folderRepository.GetFolderByNameAsync(folderDto.Name, folderDto.ParentId) != null)
        {
            throw new ArgumentException(nameof(folderDto.Name));
        }
    }
}

public interface IFolderService
{
    Task<FolderDTO> GetFolderAsync(string? path);
    Task<IEnumerable<FolderDTO>> GetRootFoldersAsync();
    Task<int> AddFolderAsync(FolderDTO folderDto);
}

public interface IFolderRepository
{
    Task<IEnumerable<FolderEntity>> GetRootFoldersAsync();
    Task<FolderEntity?> GetFolderByNameAsync(string name, int? parentId = null);
    Task<int> AddFolderAsync(FolderEntity folderEntity);
}

public class FolderDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    public IEnumerable<FolderDTO> SubFolders { get; set; } = new List<FolderDTO>();
}

public class FolderEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public FolderEntity? Parent { get; set; }

    public int? ParentId { get; set; }

    public IEnumerable<FolderEntity> SubFolders { get; set; } = new List<FolderEntity>();
}

public class FolderNotFoundException : Exception
{
    public FolderNotFoundException()
        : base()
    {
    }

    public FolderNotFoundException(string message)
        : base(message)
    {
    }

    public FolderNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
