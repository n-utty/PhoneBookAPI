using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhoneBookAPI.Controllers.Contacts.Dtos;
using PhoneBookAPI.Infrastructue;
using PhoneBookAPI.Models;
using PhoneBookAPI.Models.Contacts;

namespace PhoneBookAPI.Controllers.Contacts
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IRepository<Contact> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(
            IRepository<Contact> repository,
            IMapper mapper,
            ILogger<ContactsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactResponseDto>>> GetAllContacts()
        {
            try
            {
                var contacts = await _repository.GetAllAsync();
                var contactDtos = _mapper.Map<IEnumerable<ContactResponseDto>>(contacts);
                return Ok(contactDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts");
                return StatusCode(500, "An error occurred while retrieving contacts");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactResponseDto>> GetContact(Guid id)
        {
            try
            {
                var contact = await _repository.GetByIdAsync(id);
                if (contact == null)
                {
                    return NotFound($"Contact with ID {id} not found");
                }

                var contactDto = _mapper.Map<ContactResponseDto>(contact);
                return Ok(contactDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact with ID {ContactId}", id);
                return StatusCode(500, "An error occurred while retrieving the contact");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactResponseDto>> CreateContact([FromBody] ContactCreateDto contactDto)
        {
            try
            {
                // Check if phone number already exists
                var existingContact = await _repository.SearchAsync(c => c.PhoneNumber == contactDto.PhoneNumber);
                if (existingContact.Any())
                {
                    return BadRequest("Phone number already exists");
                }

                var contact = _mapper.Map<Contact>(contactDto);
                var createdContact = await _repository.AddAsync(contact);
                var responseDto = _mapper.Map<ContactResponseDto>(createdContact);

                return CreatedAtAction(
                    nameof(GetContact),
                    new { id = responseDto.Id },
                    responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                return StatusCode(500, "An error occurred while creating the contact");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactResponseDto>> UpdateContact(Guid id, [FromBody] ContactUpdateDto contactDto)
        {
            try
            {
                var existingContact = await _repository.GetByIdAsync(id);
                if (existingContact == null)
                {
                    return NotFound($"Contact with ID {id} not found");
                }

                // Check if phone number is already used by another contact
                var phoneNumberExists = await _repository.SearchAsync(
                    c => c.PhoneNumber == contactDto.PhoneNumber && c.Id != id);
                if (phoneNumberExists.Any())
                {
                    return BadRequest("Phone number already exists for another contact");
                }

                _mapper.Map(contactDto, existingContact);
                var updatedContact = await _repository.UpdateAsync(existingContact);
                var responseDto = _mapper.Map<ContactResponseDto>(updatedContact);

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact with ID {ContactId}", id);
                return StatusCode(500, "An error occurred while updating the contact");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            try
            {
                var contact = await _repository.GetByIdAsync(id);
                if (contact == null)
                {
                    return NotFound($"Contact with ID {id} not found");
                }

                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);
                return StatusCode(500, "An error occurred while deleting the contact");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ContactResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactResponseDto>>> SearchContacts([FromQuery] ContactSearchDto searchDto)
        {
            try
            {
                var contacts = await _repository.SearchAsync(c =>
                    string.IsNullOrEmpty(searchDto.SearchTerm) ||
                    c.Name.Contains(searchDto.SearchTerm) ||
                    c.PhoneNumber.Contains(searchDto.SearchTerm));

                var contactDtos = _mapper.Map<IEnumerable<ContactResponseDto>>(contacts);
                return Ok(contactDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching contacts with term {SearchTerm}", searchDto.SearchTerm);
                return StatusCode(500, "An error occurred while searching contacts");
            }
        }
    }
}