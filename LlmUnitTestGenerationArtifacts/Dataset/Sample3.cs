using System.Linq.Expressions;

namespace Dataset.Sample3;

public class NoteService : INoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<NoteEntity> _notes;

    public NoteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _notes = _unitOfWork.GetRepository<NoteEntity>();
    }

    public async Task<ICollection<NoteModel>> GetAllByUserIdAsync(int userId)
    {
        var noteEntityList = await _notes.GetAllAsync(n => n.UserId == userId);
        return noteEntityList.Select(e => MapEntityToModel(e)).ToList();
    }

    public async Task<NoteModel> GetByIdAsync(int id)
    {
        var noteEntity = await _notes.GetByIdAsync(id)
            ?? throw new NoteNotFoundException();
        return MapEntityToModel(noteEntity);
    }

    public async Task<int> AddAsync(NoteModel noteModel)
    {
        if (noteModel == null || noteModel.Content == null)
        {
            throw new NoteNotFoundException();
        }

        if (noteModel.UserId <= 0)
        {
            throw new UserNotFoundException();
        }

        noteModel.Id = 0;
        noteModel.Title = noteModel.Title.Trim();
        noteModel.CreatedAt = DateTime.UtcNow;
        noteModel.ModifiedAt = DateTime.UtcNow;

        var noteEntity = MapModelToEntity(noteModel);

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addedNoteEntity = await _notes.AddAsync(noteEntity);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return addedNoteEntity.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> UpdateAsync(NoteModel noteModel)
    {
        if (noteModel == null || noteModel.Content == null)
        {
            throw new NoteNotFoundException();
        }

        var oldNoteEntity = await _notes.FirstOrDefaultAsNoTrackingAsync(e => e.Id == noteModel.Id)
            ?? throw new NoteNotFoundException();

        if (oldNoteEntity.UserId != noteModel.UserId)
        {
            throw new UserNotFoundException();
        }

        var newNoteEntity = MapModelToEntity(noteModel);

        newNoteEntity.Title = newNoteEntity.Title.Trim();
        newNoteEntity.CreatedAt = oldNoteEntity.CreatedAt;
        newNoteEntity.ModifiedAt = DateTime.UtcNow;

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var updatedNoteEntity = _notes.Update(newNoteEntity);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return updatedNoteEntity.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        var noteEntity = await _notes.GetByIdAsync(id)
            ?? throw new NoteNotFoundException();

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deletedNoteEntity = _notes.Delete(noteEntity);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return deletedNoteEntity.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private NoteModel MapEntityToModel(NoteEntity noteEntity)
    {
        return new NoteModel()
        {
            Id = noteEntity.Id,
            Title = noteEntity.Title,
            Content = noteEntity.Content,
            CreatedAt = noteEntity.CreatedAt,
            ModifiedAt = noteEntity.ModifiedAt,
            UserId = noteEntity.UserId
        };
    }

    private NoteEntity MapModelToEntity(NoteModel noteModel)
    {
        return new NoteEntity()
        {
            Id = noteModel.Id,
            Title = noteModel.Title,
            Content = noteModel.Content,
            CreatedAt = noteModel.CreatedAt,
            ModifiedAt = noteModel.ModifiedAt,
            UserId = noteModel.UserId
        };
    }
}

public interface INoteService
{
    Task<ICollection<NoteModel>> GetAllByUserIdAsync(int userId);
    Task<NoteModel> GetByIdAsync(int id);
    Task<int> AddAsync(NoteModel noteModel);
    Task<int> UpdateAsync(NoteModel noteModel);
    Task<int> DeleteByIdAsync(int id);
}

public interface IUnitOfWork : IDisposable
{
    Task SaveAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;
}

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<ICollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    TEntity Update(TEntity entity);
    TEntity Delete(TEntity entity);
    Task<TEntity?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
}

public interface IDbContextTransaction : IDisposable, IAsyncDisposable
{
    Guid TransactionId { get; }
    void Commit();
    Task CommitAsync(CancellationToken cancellationToken = default);
    void Rollback();
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public class NoteModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int UserId { get; set; }
}

public class NoteEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int UserId { get; set; }
}

public class NoteNotFoundException : Exception
{
}

public class UserNotFoundException : Exception
{
}
