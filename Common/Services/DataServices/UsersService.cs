using Models;
using MongoDB.Bson;

namespace Services.DataServices
{
    public class UsersService
    {

        internal RealmService _realmService;

        public UsersService(RealmService realmService)
        {
            _realmService = realmService;
        }

        public User? GetUserById(ObjectId? userId)
        {
            if (userId == null) return null;

            return _realmService.Realm!.All<User>()
                .FirstOrDefault(record => record.Id == userId);
        }

        public List<User> ListUsers()
        {
            var users = _realmService.Realm.All<User>()
                .OrderBy(record => record.Name)
                .ToList();
            return users;
        }

    }
}
