using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);

        Task<AppUser> getUserWithLikes(int userId);

        Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}