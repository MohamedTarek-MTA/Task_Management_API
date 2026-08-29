using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserDTO ToDTO(User user);
        public partial User ToEntity(UserDTO userDTO);
    }
}
