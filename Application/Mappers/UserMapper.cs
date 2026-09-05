using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs.UserDTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserDTO ToDTO(User user);
        public partial User ToEntity(UserDTO userDTO);

        public partial User ToEntity(CreateUserDTO createUserDTO);
        public partial CreateUserDTO ToCreationDTO(User user);
        public partial void Map(UpdateUserDTO userDTO, [MappingTarget] User user);
    }
}
