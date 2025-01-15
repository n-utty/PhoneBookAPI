using AutoMapper;
using PhoneBookAPI.Controllers.Contacts.Dtos;
using PhoneBookAPI.Models.Contacts;

namespace PhoneBookAPI.Mappings
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<Contact, ContactResponseDto>();
            CreateMap<ContactCreateDto, Contact>();
            CreateMap<ContactUpdateDto, Contact>()
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}