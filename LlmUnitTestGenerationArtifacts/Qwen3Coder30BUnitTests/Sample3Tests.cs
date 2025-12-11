using System.Linq.Expressions;
using Dataset.Sample3;
using NSubstitute;

namespace Qwen3Coder30BUnitTests
{
    public class NoteServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<NoteEntity> _notesRepository;
        private readonly NoteService _service;

        public NoteServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _notesRepository = Substitute.For<IRepository<NoteEntity>>();
            _unitOfWork.GetRepository<NoteEntity>().Returns(_notesRepository);
            _service = new NoteService(_unitOfWork);
        }

        [Fact]
        public async Task GetAllByUserIdAsync_WithValidUserId_ReturnsNotes()
        {
            // Arrange
            var userId = 1;
            var noteEntities = new List<NoteEntity>
            {
                new NoteEntity { Id = 1, Title = "Title1", Content = "Content1", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = userId },
                new NoteEntity { Id = 2, Title = "Title2", Content = "Content2", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = userId }
            };
            _notesRepository.GetAllAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(noteEntities);

            // Act
            var result = await _service.GetAllByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Title1", result.First().Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsNote()
        {
            // Arrange
            var noteEntity = new NoteEntity { Id = 1, Title = "Title", Content = "Content", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = 1 };
            _notesRepository.GetByIdAsync(1).Returns(noteEntity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Equal("Title", result.Title);
            Assert.Equal("Content", result.Content);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNoteNotFoundException()
        {
            // Arrange
            _notesRepository.GetByIdAsync(1).Returns((NoteEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.GetByIdAsync(1));
        }

        [Fact]
        public async Task AddAsync_WithValidModel_AddsAndReturnsId()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Title", Content = "Content", UserId = 1 };
            var noteEntity = new NoteEntity { Id = 1, Title = "Title", Content = "Content", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = 1 };
            _notesRepository.AddAsync(Arg.Any<NoteEntity>()).Returns(noteEntity);
            _unitOfWork.BeginTransactionAsync().Returns(Substitute.For<IDbContextTransaction>());

            // Act
            var result = await _service.AddAsync(noteModel);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task AddAsync_WithNullModel_ThrowsNoteNotFoundException()
        {
            // Arrange
            NoteModel noteModel = null;

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.AddAsync(noteModel));
        }

        [Fact]
        public async Task AddAsync_WithNullContent_ThrowsNoteNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Title", Content = null, UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.AddAsync(noteModel));
        }

        [Fact]
        public async Task AddAsync_WithInvalidUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Title", Content = "Content", UserId = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _service.AddAsync(noteModel));
        }

        [Fact]
        public async Task UpdateAsync_WithValidModel_UpdatesAndReturnsId()
        {
            // Arrange
            var oldNoteEntity = new NoteEntity { Id = 1, Title = "Old Title", Content = "Content", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = 1 };
            var noteModel = new NoteModel { Id = 1, Title = "New Title", Content = "Content", UserId = 1 };
            _notesRepository.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(oldNoteEntity);
            _notesRepository.Update(Arg.Any<NoteEntity>()).Returns(x => x.Arg<NoteEntity>());

            // Act
            var result = await _service.UpdateAsync(noteModel);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAsync_WithNullModel_ThrowsNoteNotFoundException()
        {
            // Arrange
            NoteModel noteModel = null;

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.UpdateAsync(noteModel));
        }

        [Fact]
        public async Task UpdateAsync_WithNullContent_ThrowsNoteNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Id = 1, Title = "Title", Content = null, UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.UpdateAsync(noteModel));
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentNote_ThrowsNoteNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Id = 1, Title = "Title", Content = "Content", UserId = 1 };
            _notesRepository.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns((NoteEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.UpdateAsync(noteModel));
        }

        [Fact]
        public async Task UpdateAsync_WithUserMismatch_ThrowsUserNotFoundException()
        {
            // Arrange
            var oldNoteEntity = new NoteEntity { Id = 1, Title = "Title", Content = "Content", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = 1 };
            var noteModel = new NoteModel { Id = 1, Title = "Title", Content = "Content", UserId = 2 };
            _notesRepository.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(oldNoteEntity);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _service.UpdateAsync(noteModel));
        }

        [Fact]
        public async Task DeleteByIdAsync_WithValidId_DeletesAndReturnsId()
        {
            // Arrange
            var noteEntity = new NoteEntity { Id = 1, Title = "Title", Content = "Content", CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, UserId = 1 };
            _notesRepository.GetByIdAsync(1).Returns(noteEntity);
            _notesRepository.Delete(Arg.Any<NoteEntity>()).Returns(noteEntity);
            _unitOfWork.BeginTransactionAsync().Returns(Substitute.For<IDbContextTransaction>());

            // Act
            var result = await _service.DeleteByIdAsync(1);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteByIdAsync_WithInvalidId_ThrowsNoteNotFoundException()
        {
            // Arrange
            _notesRepository.GetByIdAsync(1).Returns((NoteEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _service.DeleteByIdAsync(1));
        }
    }
}
