using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTest.Functionality
{
    public record User(string firstName, string lastName)
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string Phone { get; set; }
        public bool VerifiedEmail { get; set; } = false;

    }
    public class UserManagement
    {
        private readonly List<User> _users = new();

        private int idCounter = 1;

        public IEnumerable<User> AllUsers => _users;

        public void AddUser(User user)
        {
            _users.Add(user with { Id = idCounter++ });
        }

        public void UpdatePhone(User user)
        {
            var userDb = _users.First(u => u.Id == user.Id);
            userDb.Phone = user.Phone;
        }

        public void VerifyEmail(int userId)
        {
            var userDb = _users.First(u => u.Id == userId);

            userDb.VerifiedEmail = true;
     
        }
    }
}
