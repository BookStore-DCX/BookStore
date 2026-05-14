using AutoMapper;
using BookStore.DTOs.State;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests
{
    public class StateServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IStateRepository> _stateRepoMock;
        private readonly StateService _service;

        public StateServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _stateRepoMock = new Mock<IStateRepository>();
            _uowMock.Setup(u => u.States).Returns(_stateRepoMock.Object);
            _service = new StateService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllStatesAsync_ReturnsAllStates()
        {
            var states = new List<State>
            {
                new State { StateCode = "CA", StateName = "California" }
            };
            var dtos = new List<StateDto>
            {
                new StateDto { StateCode = "CA", StateName = "California" }
            };
            _stateRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(states);
            _mapperMock.Setup(m => m.Map<IEnumerable<StateDto>>(states)).Returns(dtos);

            var result = await _service.GetAllStatesAsync();

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetStateByCodeAsync_ValidCode_ReturnsStateDto()
        {
            var state = new State { StateCode = "CA", StateName = "California" };
            var dto = new StateDto { StateCode = "CA", StateName = "California" };
            _stateRepoMock.Setup(r => r.GetByIdAsync("CA")).ReturnsAsync(state);
            _mapperMock.Setup(m => m.Map<StateDto>(state)).Returns(dto);

            var result = await _service.GetStateByCodeAsync("CA");

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetAllStatesAsync_EmptyList_ReturnsEmptyCollection()
        {
            _stateRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<State>());
            _mapperMock.Setup(m => m.Map<IEnumerable<StateDto>>(It.IsAny<IEnumerable<State>>()))
                       .Returns(new List<StateDto>());

            var result = await _service.GetAllStatesAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStateByCodeAsync_ValidCode_MapsCorrectly()
        {
            var state = new State { StateCode = "TX", StateName = "Texas" };
            var dto = new StateDto { StateCode = "TX", StateName = "Texas" };
            _stateRepoMock.Setup(r => r.GetByIdAsync("TX")).ReturnsAsync(state);
            _mapperMock.Setup(m => m.Map<StateDto>(state)).Returns(dto);

            var result = await _service.GetStateByCodeAsync("TX");

            Assert.Equal("TX", result.StateCode);
        }

        [Fact]
        public async Task GetStateByCodeAsync_InvalidCode_ThrowsNotFoundException()
        {
            _stateRepoMock.Setup(r => r.GetByIdAsync("ZZ")).ReturnsAsync((State)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetStateByCodeAsync("ZZ"));
        }

        [Fact]
        public async Task GetStateByCodeAsync_EmptyString_ThrowsNotFoundException()
        {
            _stateRepoMock.Setup(r => r.GetByIdAsync("")).ReturnsAsync((State)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetStateByCodeAsync(""));
        }

        [Fact]
        public async Task GetStateByCodeAsync_NullCode_ThrowsNotFoundException()
        {
            _stateRepoMock.Setup(r => r.GetByIdAsync(null!)).ReturnsAsync((State)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetStateByCodeAsync(null!));
        }

        [Fact]
        public async Task GetStateByCodeAsync_LowercaseCode_ThrowsNotFoundException()
        {
            _stateRepoMock.Setup(r => r.GetByIdAsync("ca")).ReturnsAsync((State)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetStateByCodeAsync("ca"));
        }
    }
}