using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.DTO.User
{
    public record CurrentUserResponseDTO(
            string Id 
            , string FullName
            , string Email 
            , string Phone 
            , bool IsVerified 
            , DateTime CreatedAt
        
        );
}
