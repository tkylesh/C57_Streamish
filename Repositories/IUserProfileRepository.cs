using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile userProfile);
        void Delete(int id);
        List<UserProfile> GetAll();
        UserProfile GetById(int Id);
        UserProfile GetUserProfileByIdWithComments(int Id);
        void Update(UserProfile userProfile);
    }
}