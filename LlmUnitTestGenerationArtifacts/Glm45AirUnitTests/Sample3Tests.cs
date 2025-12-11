using System.Linq.Expressions;
using Dataset.Sample3;
using NSubstitute;

namespace Glm45AirUnitTests;

public class NoteServiceTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IRepository<NoteEntity> _repositoryMock;
    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _repositoryMock = Substitute.For<IRepository<NoteEntity>>();
        _unitOfWorkMock.GetRepository<NoteEntity>().Returns(_repositoryMock);
        _noteService = new NoteService(_unitOfWorkMock);
    }

    public class GetAllByUserIdAsync : NoteServiceTests
    {
        [Fact]
        public async Task GivenValidUserId_WhenCalled_ReturnsNoteModels()
        {
            // Arrange
            int userId = 1;
            var noteEntities = new List<NoteEntity>
            {
                new NoteEntity { Id = 1, Title = "Test1", Content = "Content1", UserId = userId },
                new NoteEntity { Id = 2, Title = "Test2", Content = "Content2", UserId = userId }
            };
            _repositoryMock.GetAllAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>())
                .Returns(noteEntities);

            // Act
            var result = await _noteService.GetAllByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(noteEntities[0].Id, result.ElementAt(0).Id);
            Assert.Equal(noteEntities[0].Title, result.ElementAt(0).Title);
            Assert.Equal(noteEntities[0].Content, result.ElementAt(0).Content);
            Assert.Equal(noteEntities[0].UserId, result.ElementAt(0).UserId);
            Assert.Equal(noteEntities[1].Id, result.ElementAt(1).Id);
            Assert.Equal(noteEntities[1].Title, result.ElementAt(1).Title);
            Assert.Equal(noteEntities[1].Content, result.ElementAt(1).Content);
            Assert.Equal(noteEntities[1].UserId, result.ElementAt(1).UserId);
            await _repositoryMock.Received(1).GetAllAsync(Arg.Is<Expression<Func<NoteEntity, bool>>>(e => e.Compile().Invoke(new NoteEntity { UserId = userId })));
        }

        [Fact]
        public async Task GivenNoNotesForUserId_WhenCalled_ReturnsEmptyCollection()
        {
            // Arrange
            int userId = 1;
            _repositoryMock.GetAllAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>())
                .Returns(new List<NoteEntity>());

            // Act
            var result = await _noteService.GetAllByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await _repositoryMock.Received(1).GetAllAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>());
        }
    }

    public class GetByIdAsync : NoteServiceTests
    {
        [Fact]
        public async Task GivenExistingId_WhenCalled_ReturnsNoteModel()
        {
            // Arrange
            int id = 1;
            var noteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1 };
            _repositoryMock.GetByIdAsync(id).Returns(noteEntity);

            // Act
            var result = await _noteService.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(noteEntity.Title, result.Title);
            Assert.Equal(noteEntity.Content, result.Content);
            Assert.Equal(noteEntity.UserId, result.UserId);
            await _repositoryMock.Received(1).GetByIdAsync(id);
        }

        [Fact]
        public async Task GivenNonExistingId_WhenCalled_ThrowsNoteNotFoundException()
        {
            // Arrange
            int id = 1;
            _repositoryMock.GetByIdAsync(id).Returns((NoteEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _noteService.GetByIdAsync(id));
            await _repositoryMock.Received(1).GetByIdAsync(id);
        }
    }

    public class AddAsync : NoteServiceTests
    {
        [Fact]
        public async Task GivenValidNoteModel_WhenCalled_ReturnsNewId()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Test", Content = "Content", UserId = 1 };
            var noteEntity = new NoteEntity { Id = 1, Title = "Test", Content = "Content", UserId = 1 };
            var addedNoteEntity = new NoteEntity { Id = 2, Title = "Test", Content = "Content", UserId = 1 };
            _repositoryMock.AddAsync(noteEntity).Returns(addedNoteEntity);
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.SaveAsync().Returns(Task.CompletedTask);

            // Act
            var result = await _noteService.AddAsync(noteModel);

            // Assert
            Assert.Equal(addedNoteEntity.Id, result);
            await _unitOfWorkMock.Received(1).BeginTransactionAsync();
            await _unitOfWorkMock.Received(1).SaveAsync();
            await transactionMock.Received(1).CommitAsync();
            Assert.Equal(0, noteModel.Id);
            Assert.Equal(DateTime.UtcNow.Date, noteModel.CreatedAt.Date);
            Assert.Equal(DateTime.UtcNow.Date, noteModel.ModifiedAt.Date);
            Assert.Equal("Test", noteModel.Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GivenNoteModelWithNullContent_WhenCalled_ThrowsNoteNotFoundException(string content)
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Test", Content = content, UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _noteService.AddAsync(noteModel));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task GivenNoteModelWithInvalidUserId_WhenCalled_ThrowsUserNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Test", Content = "Content", UserId = -1 };

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _noteService.AddAsync(noteModel));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task WhenSaveFails_RollsBackTransaction()
        {
            // Arrange
            var noteModel = new NoteModel { Title = "Test", Content = "Content", UserId = 1 };
            var noteEntity = new NoteModel { Id = 0, Title = "Test", Content = "Content", UserId = 1 };
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.When(u => u.SaveAsync()).Do(_ => throw new Exception("Save failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _noteService.AddAsync(noteModel));
            await transactionMock.Received(1).RollbackAsync();
            await transactionMock.DidNotReceive().CommitAsync();
        }
    }

    public class UpdateAsync : NoteServiceTests
    {
        [Fact]
        public async Task GivenValidNoteModel_WhenCalled_ReturnsUpdatedId()
        {
            // Arrange
            int id = 1;
            var noteModel = new NoteModel { Id = id, Title = "Test", Content = "Content", UserId = 1, CreatedAt = new DateTime(2023, 1, 1) };
            var oldNoteEntity = new NoteEntity { Id = id, Title = "Old", Content = "OldContent", UserId = 1, CreatedAt = new DateTime(2023, 1, 1) };
            var newNoteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1, CreatedAt = new DateTime(2023, 1, 1), ModifiedAt = DateTime.UtcNow };
            var updatedNoteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1, CreatedAt = new DateTime(2023, 1, 1), ModifiedAt = DateTime.UtcNow };
            _repositoryMock.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(oldNoteEntity);
            _repositoryMock.Update(newNoteEntity).Returns(updatedNoteEntity);
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.SaveAsync().Returns(Task.CompletedTask);

            // Act
            var result = await _noteService.UpdateAsync(noteModel);

            // Assert
            Assert.Equal(id, result);
            Assert.Equal(oldNoteEntity.CreatedAt, noteModel.CreatedAt);
            Assert.Equal(DateTime.UtcNow.Date, noteModel.ModifiedAt.Date);
            Assert.Equal("Test", noteModel.Title);
            await _unitOfWorkMock.Received(1).BeginTransactionAsync();
            await _unitOfWorkMock.Received(1).SaveAsync();
            await transactionMock.Received(1).CommitAsync();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GivenNoteModelWithNullContent_WhenCalled_ThrowsNoteNotFoundException(string content)
        {
            // Arrange
            var noteModel = new NoteModel { Id = 1, Title = "Test", Content = content, UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _noteService.UpdateAsync(noteModel));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task GivenNonExistingNoteId_WhenCalled_ThrowsNoteNotFoundException()
        {
            // Arrange
            var noteModel = new NoteModel { Id = 1, Title = "Test", Content = "Content", UserId = 1 };
            _repositoryMock.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns((NoteEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _noteService.UpdateAsync(noteModel));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task GivenUserIdMismatch_WhenCalled_ThrowsUserNotFoundException()
        {
            // Arrange
            int id = 1;
            var noteModel = new NoteModel { Id = id, Title = "Test", Content = "Content", UserId = 2 };
            var oldNoteEntity = new NoteEntity { Id = id, Title = "Old", Content = "OldContent", UserId = 1 };
            _repositoryMock.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(oldNoteEntity);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _noteService.UpdateAsync(noteModel));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task WhenSaveFails_RollsBackTransaction()
        {
            // Arrange
            int id = 1;
            var noteModel = new NoteModel { Id = id, Title = "Test", Content = "Content", UserId = 1, CreatedAt = new DateTime(2023, 1, 1) };
            var oldNoteEntity = new NoteEntity { Id = id, Title = "Old", Content = "OldContent", UserId = 1, CreatedAt = new DateTime(2023, 1, 1) };
            _repositoryMock.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(oldNoteEntity);
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.When(u => u.SaveAsync()).Do(_ => throw new Exception("Save failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _noteService.UpdateAsync(noteModel));
            await transactionMock.Received(1).RollbackAsync();
            await transactionMock.DidNotReceive().CommitAsync();
        }
    }

    public class DeleteByIdAsync : NoteServiceTests
    {
        [Fact]
        public async Task GivenExistingId_WhenCalled_ReturnsDeletedId()
        {
            // Arrange
            int id = 1;
            var noteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1 };
            var deletedNoteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1 };
            _repositoryMock.GetByIdAsync(id).Returns(noteEntity);
            _repositoryMock.Delete(noteEntity).Returns(deletedNoteEntity);
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.SaveAsync().Returns(Task.CompletedTask);

            // Act
            var result = await _noteService.DeleteByIdAsync(id);

            // Assert
            Assert.Equal(id, result);
            await _unitOfWorkMock.Received(1).BeginTransactionAsync();
            await _unitOfWorkMock.Received(1).SaveAsync();
            await transactionMock.Received(1).CommitAsync();
        }

        [Fact]
        public async Task GivenNonExistingId_WhenCalled_ThrowsNoteNotFoundException()
        {
            // Arrange
            int id = 1;
            _repositoryMock.GetByIdAsync(id).Returns((NoteEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(async () => await _noteService.DeleteByIdAsync(id));
            await _unitOfWorkMock.DidNotReceive().BeginTransactionAsync();
            await _unitOfWorkMock.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task WhenSaveFails_RollsBackTransaction()
        {
            // Arrange
            int id = 1;
            var noteEntity = new NoteEntity { Id = id, Title = "Test", Content = "Content", UserId = 1 };
            _repositoryMock.GetByIdAsync(id).Returns(noteEntity);
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _unitOfWorkMock.BeginTransactionAsync().Returns(Task.FromResult(transactionMock));
            _unitOfWorkMock.When(u => u.SaveAsync()).Do(_ => throw new Exception("Save failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _noteService.DeleteByIdAsync(id));
            await transactionMock.Received(1).RollbackAsync();
            await transactionMock.DidNotReceive().CommitAsync();
        }
    }
}
