using Models;

namespace Services.DataServices
{
    public class UsersService
    {

        internal RealmService _realmService;

        public UsersService(RealmService realmService)
        {
            _realmService = realmService;
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
