using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    void Add(MemberLike like);
    void Delete(MemberLike like);
    Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
    Task<PaginationResult<Member>> GetMemberLikes(LikesRequest likesRequest);
    Task<bool> SaveAllChanges();
}