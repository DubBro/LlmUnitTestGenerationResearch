using System.Linq.Expressions;
using Dataset.Sample3;
using NSubstitute;

namespace Gpt5MiniUnitTests
{
    public class NoteServiceTests
    {
        [Fact]
        public async Task GetAllByUserIdAsync_WhenCalled_ReturnsMappedNoteModels()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var entities = new List<NoteEntity>
            {
                new NoteEntity { Id = 1, Title = "T1", Content = "C1", UserId = 10, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new NoteEntity { Id = 2, Title = "T2", Content = "C2", UserId = 10, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow }
            };

            Expression<Func<NoteEntity, bool>>? capturedPredicate = null;
            notesRepo.GetAllAsync(Arg.Do<Expression<Func<NoteEntity, bool>>>(p => capturedPredicate = p))
                .Returns(Task.FromResult((ICollection<NoteEntity>)entities));

            var service = new NoteService(unitOfWork);

            // Act
            var result = await service.GetAllByUserIdAsync(10);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Id == 1 && m.Title == "T1" && m.Content == "C1" && m.UserId == 10);
            Assert.Contains(result, m => m.Id == 2 && m.Title == "T2" && m.Content == "C2" && m.UserId == 10);
            Assert.NotNull(capturedPredicate);
            var predicate = capturedPredicate!.Compile();
            Assert.True(predicate(new NoteEntity { UserId = 10 }));
            Assert.False(predicate(new NoteEntity { UserId = 99 }));
        }

        [Fact]
        public async Task GetByIdAsync_WhenNoteExists_ReturnsMappedModel()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var entity = new NoteEntity { Id = 5, Title = "Title", Content = "Body", UserId = 3, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
            notesRepo.GetByIdAsync(5).Returns(Task.FromResult<NoteEntity?>(entity));

            var service = new NoteService(unitOfWork);

            // Act
            var result = await service.GetByIdAsync(5);

            // Assert
            Assert.Equal(5, result.Id);
            Assert.Equal("Title", result.Title);
            Assert.Equal("Body", result.Content);
            Assert.Equal(3, result.UserId);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
            Assert.Equal(entity.ModifiedAt, result.ModifiedAt);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            notesRepo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<NoteEntity?>(null));

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.GetByIdAsync(999));
        }

        [Fact]
        public async Task AddAsync_NullModel_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.AddAsync(null!));
        }

        [Fact]
        public async Task AddAsync_NullContent_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var model = new NoteModel { Id = 1, Title = "X", Content = null!, UserId = 1 };

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.AddAsync(model));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task AddAsync_InvalidUserId_ThrowsUserNotFoundException(int invalidUserId)
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var model = new NoteModel { Id = 1, Title = "Title", Content = "Content", UserId = invalidUserId };

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => service.AddAsync(model));
        }

        [Fact]
        public async Task AddAsync_WhenRepositorySucceeds_ReturnsNewIdAndCommits()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            unitOfWork.SaveAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            transaction.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            NoteEntity? capturedEntity = null;
            var addedEntity = new NoteEntity { Id = 42, Title = "Trimmed", Content = "C", UserId = 2 };
            notesRepo.AddAsync(Arg.Do<NoteEntity>(e => capturedEntity = e)).Returns(Task.FromResult(addedEntity));

            var service = new NoteService(unitOfWork);

            var model = new NoteModel { Id = 10, Title = "  Trimmed  ", Content = "C", UserId = 2 };

            // Act
            var resultId = await service.AddAsync(model);

            // Assert
            Assert.Equal(42, resultId);
            Assert.NotNull(capturedEntity);
            Assert.Equal(0, capturedEntity!.Id);
            Assert.Equal("Trimmed", capturedEntity.Title);
            Assert.Equal("C", capturedEntity.Content);
            Assert.Equal(2, capturedEntity.UserId);
            Assert.NotEqual(default, capturedEntity.CreatedAt);
            Assert.Equal(capturedEntity.CreatedAt, capturedEntity.ModifiedAt);

            await unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
            await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AddAsync_WhenAddThrows_RollsBackAndPropagates()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            notesRepo.AddAsync(Arg.Any<NoteEntity>()).Returns<Task<NoteEntity>>(ci => Task.FromException<NoteEntity>(new InvalidOperationException("db fail")));
            transaction.RollbackAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var service = new NoteService(unitOfWork);

            var model = new NoteModel { Id = 0, Title = "Title", Content = "C", UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(model));
            await transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_NullModel_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.UpdateAsync(null!));
        }

        [Fact]
        public async Task UpdateAsync_NullContent_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var model = new NoteModel { Id = 1, Title = "t", Content = null!, UserId = 1 };

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.UpdateAsync(model));
        }

        [Fact]
        public async Task UpdateAsync_OldNoteNotFound_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            notesRepo.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>())
                .Returns(Task.FromResult<NoteEntity?>(null));

            var service = new NoteService(unitOfWork);

            var model = new NoteModel { Id = 1, Title = "t", Content = "c", UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.UpdateAsync(model));
        }

        [Fact]
        public async Task UpdateAsync_UserMismatch_ThrowsUserNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            var old = new NoteEntity { Id = 1, Title = "old", Content = "c", UserId = 2, CreatedAt = new DateTime(2000, 1, 1), ModifiedAt = new DateTime(2000,1,1) };
            notesRepo.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>())
                .Returns(Task.FromResult<NoteEntity?>(old));

            var service = new NoteService(unitOfWork);

            var model = new NoteModel { Id = 1, Title = "new", Content = "c", UserId = 99 };

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => service.UpdateAsync(model));
        }

        [Fact]
        public async Task UpdateAsync_Success_ReturnsUpdatedIdAndCommits()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            unitOfWork.SaveAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            transaction.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var oldEntity = new NoteEntity { Id = 7, Title = "Old", Content = "OldC", UserId = 3, CreatedAt = new DateTime(2020, 1, 1), ModifiedAt = new DateTime(2020, 1, 1) };
            notesRepo.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(Task.FromResult<NoteEntity?>(oldEntity));

            NoteEntity? capturedForUpdate = null;
            var returnedEntity = new NoteEntity { Id = 7, Title = "New", Content = "NewC", UserId = 3, CreatedAt = oldEntity.CreatedAt, ModifiedAt = DateTime.UtcNow };
            notesRepo.Update(Arg.Do<NoteEntity>(e => capturedForUpdate = e)).Returns(returnedEntity);

            var service = new NoteService(unitOfWork);

            var model = new NoteModel { Id = 7, Title = "  New  ", Content = "NewC", UserId = 3 };

            // Act
            var resultId = await service.UpdateAsync(model);

            // Assert
            Assert.Equal(7, resultId);
            Assert.NotNull(capturedForUpdate);
            Assert.Equal(oldEntity.CreatedAt, capturedForUpdate!.CreatedAt);
            Assert.Equal("New", capturedForUpdate.Title);
            Assert.Equal(oldEntity.UserId, capturedForUpdate.UserId);
            Assert.NotEqual(default, capturedForUpdate.ModifiedAt);
            await unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
            await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_WhenUpdateThrows_RollsBackAndPropagates()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            var oldEntity = new NoteEntity { Id = 8, Title = "Old", Content = "C", UserId = 1, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
            notesRepo.FirstOrDefaultAsNoTrackingAsync(Arg.Any<Expression<Func<NoteEntity, bool>>>()).Returns(Task.FromResult<NoteEntity?>(oldEntity));
            notesRepo.Update(Arg.Any<NoteEntity>()).Returns(ci => { throw new InvalidOperationException("update failed"); });
            transaction.RollbackAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var service = new NoteService(unitOfWork);
            var model = new NoteModel { Id = 8, Title = "New", Content = "C", UserId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(model));
            await transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteByIdAsync_NotFound_ThrowsNoteNotFoundException()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);

            notesRepo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<NoteEntity?>(null));

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(() => service.DeleteByIdAsync(123));
        }

        [Fact]
        public async Task DeleteByIdAsync_Success_ReturnsDeletedIdAndCommits()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            unitOfWork.SaveAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            transaction.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var entity = new NoteEntity { Id = 55, Title = "X", Content = "C", UserId = 1 };
            notesRepo.GetByIdAsync(55).Returns(Task.FromResult<NoteEntity?>(entity));
            notesRepo.Delete(entity).Returns(entity);

            var service = new NoteService(unitOfWork);

            // Act
            var deletedId = await service.DeleteByIdAsync(55);

            // Assert
            Assert.Equal(55, deletedId);
            await unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
            await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenDeleteThrows_RollsBackAndPropagates()
        {
            // Arrange
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var notesRepo = Substitute.For<IRepository<NoteEntity>>();
            var transaction = Substitute.For<IDbContextTransaction>();

            unitOfWork.GetRepository<NoteEntity>().Returns(notesRepo);
            unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(transaction));
            var entity = new NoteEntity { Id = 66, Title = "X", Content = "C", UserId = 1 };
            notesRepo.GetByIdAsync(66).Returns(Task.FromResult<NoteEntity?>(entity));
            notesRepo.Delete(entity).Returns(ci => { throw new InvalidOperationException("delete failed"); });
            transaction.RollbackAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            var service = new NoteService(unitOfWork);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteByIdAsync(66));
            await transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        }
    }
}
