using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PhoneBookAPI.Controllers;
using PhoneBookAPI.Controllers.Contacts.Dtos;
using PhoneBookAPI.Controllers.Contacts;
using PhoneBookAPI.Infrastructue;
using PhoneBookAPI.Models;
using PhoneBookAPI.Models.Contacts;
using System.Linq.Expressions;
using Xunit;

namespace PhoneBookAPI.Tests
{
    public class ContactsControllerTests
    {
        private readonly Mock<IRepository<Contact>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ContactsController>> _mockLogger;
        private readonly ContactsController _controller;

        public ContactsControllerTests()
        {
            _mockRepo = new Mock<IRepository<Contact>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ContactsController>>();
            _controller = new ContactsController(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllContacts_ReturnsOkResult_WithListOfContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "Test1", PhoneNumber = "1234567890" },
                new Contact { Id = Guid.NewGuid(), Name = "Test2", PhoneNumber = "0987654321" }
            };
            var contactDtos = new List<ContactResponseDto>
            {
                new ContactResponseDto { Id = contacts[0].Id, Name = "Test1", PhoneNumber = "1234567890" },
                new ContactResponseDto { Id = contacts[1].Id, Name = "Test2", PhoneNumber = "0987654321" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync())
                    .ReturnsAsync(contacts);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ContactResponseDto>>(contacts))
                      .Returns(contactDtos);

            // Act
            var result = await _controller.GetAllContacts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContacts = Assert.IsAssignableFrom<IEnumerable<ContactResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedContacts.Count());
        }

        [Fact]
        public async Task GetContact_ReturnsOkResult_WhenContactExists()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var contact = new Contact { Id = contactId, Name = "Test", PhoneNumber = "1234567890" };
            var contactDto = new ContactResponseDto { Id = contactId, Name = "Test", PhoneNumber = "1234567890" };

            _mockRepo.Setup(repo => repo.GetByIdAsync(contactId))
                    .ReturnsAsync(contact);
            _mockMapper.Setup(mapper => mapper.Map<ContactResponseDto>(contact))
                      .Returns(contactDto);

            // Act
            var result = await _controller.GetContact(contactId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContact = Assert.IsType<ContactResponseDto>(okResult.Value);
            Assert.Equal(contactId, returnedContact.Id);
        }

        [Fact]
        public async Task GetContact_ReturnsNotFound_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetByIdAsync(contactId))
                    .ReturnsAsync((Contact)null);

            // Act
            var result = await _controller.GetContact(contactId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateContact_ReturnsCreatedAtAction_WhenContactIsValid()
        {
            // Arrange
            var createDto = new ContactCreateDto
            {
                Name = "Test",
                PhoneNumber = "1234567890",
                Email = "test@test.com"
            };
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                PhoneNumber = "1234567890",
                Email = "test@test.com"
            };
            var responseDto = new ContactResponseDto
            {
                Id = contact.Id,
                Name = "Test",
                PhoneNumber = "1234567890",
                Email = "test@test.com"
            };

            _mockRepo.Setup(repo => repo.SearchAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                    .ReturnsAsync(new List<Contact>());
            _mockMapper.Setup(mapper => mapper.Map<Contact>(createDto))
                      .Returns(contact);
            _mockRepo.Setup(repo => repo.AddAsync(contact))
                    .ReturnsAsync(contact);
            _mockMapper.Setup(mapper => mapper.Map<ContactResponseDto>(contact))
                      .Returns(responseDto);

            // Act
            var result = await _controller.CreateContact(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedContact = Assert.IsType<ContactResponseDto>(createdAtActionResult.Value);
            Assert.Equal("Test", returnedContact.Name);
        }

        [Fact]
        public async Task CreateContact_ReturnsBadRequest_WhenPhoneNumberExists()
        {
            // Arrange
            var createDto = new ContactCreateDto
            {
                Name = "Test",
                PhoneNumber = "1234567890",
                Email = "test@test.com"
            };
            var existingContact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = "Existing",
                PhoneNumber = "1234567890"
            };

            _mockRepo.Setup(repo => repo.SearchAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                    .ReturnsAsync(new List<Contact> { existingContact });

            // Act
            var result = await _controller.CreateContact(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateContact_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var updateDto = new ContactUpdateDto
            {
                Name = "Updated",
                PhoneNumber = "0987654321",
                Email = "updated@test.com"
            };
            var existingContact = new Contact
            {
                Id = contactId,
                Name = "Original",
                PhoneNumber = "1234567890",
                Email = "test@test.com"
            };
            var updatedContact = new Contact
            {
                Id = contactId,
                Name = "Updated",
                PhoneNumber = "0987654321",
                Email = "updated@test.com"
            };
            var responseDto = new ContactResponseDto
            {
                Id = contactId,
                Name = "Updated",
                PhoneNumber = "0987654321",
                Email = "updated@test.com"
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(contactId))
                    .ReturnsAsync(existingContact);
            _mockRepo.Setup(repo => repo.SearchAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                    .ReturnsAsync(new List<Contact>());
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Contact>()))
                    .ReturnsAsync(updatedContact);
            _mockMapper.Setup(mapper => mapper.Map<ContactResponseDto>(updatedContact))
                      .Returns(responseDto);

            // Act
            var result = await _controller.UpdateContact(contactId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContact = Assert.IsType<ContactResponseDto>(okResult.Value);
            Assert.Equal("Updated", returnedContact.Name);
        }

        [Fact]
        public async Task DeleteContact_ReturnsNoContent_WhenContactIsDeleted()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var contact = new Contact { Id = contactId, Name = "Test", PhoneNumber = "1234567890" };

            _mockRepo.Setup(repo => repo.GetByIdAsync(contactId))
                    .ReturnsAsync(contact);
            _mockRepo.Setup(repo => repo.DeleteAsync(contactId))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteContact(contactId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteContact_ReturnsNotFound_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetByIdAsync(contactId))
                    .ReturnsAsync((Contact)null);

            // Act
            var result = await _controller.DeleteContact(contactId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchContacts_ReturnsOkResult_WithMatchingContacts()
        {
            // Arrange
            var searchDto = new ContactSearchDto { SearchTerm = "Test" };
            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "Test1", PhoneNumber = "1234567890" },
                new Contact { Id = Guid.NewGuid(), Name = "Test2", PhoneNumber = "0987654321" }
            };
            var contactDtos = new List<ContactResponseDto>
            {
                new ContactResponseDto { Id = contacts[0].Id, Name = "Test1", PhoneNumber = "1234567890" },
                new ContactResponseDto { Id = contacts[1].Id, Name = "Test2", PhoneNumber = "0987654321" }
            };

            _mockRepo.Setup(repo => repo.SearchAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                    .ReturnsAsync(contacts);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ContactResponseDto>>(contacts))
                      .Returns(contactDtos);

            // Act
            var result = await _controller.SearchContacts(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContacts = Assert.IsAssignableFrom<IEnumerable<ContactResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedContacts.Count());
        }
    }
}