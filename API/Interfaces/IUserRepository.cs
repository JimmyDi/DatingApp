using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        
        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUserNameAsync(string UserName);

        Task<MemberDto> GetMemberAsync(string UserName);

        Task<PageList<MemberDto>> GetMemberAsync(UserParams userParams);

        Task<string> GetUserGender(string username);
    }
}