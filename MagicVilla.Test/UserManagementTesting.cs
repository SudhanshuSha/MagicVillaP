using SampleTest.Functionality;

namespace MagicVilla.Test
{
    public class UserManagementTesting
    {
        // we need to decorate the method uding fact 
        [Fact]
        public void Add_User()
        {
            // wrtitng test is broken into three parts
            //1 Arrange we will point out to exact function or method tha we want to test
            var userMangement = new UserManagement();
            // Act we will execut what we want
            userMangement.AddUser(new User("Sudhanshu", "Sharma"));
            // Assert compare the result with expected value 

            var savedUser = Assert.Single(userMangement.AllUsers);

            Assert.NotNull(savedUser);// checking userMnagement list is not empty
            Assert.Equal("Sudhanshu", savedUser.firstName);
            Assert.Equal("Sharma", savedUser.lastName);
            Assert.False(savedUser.VerifiedEmail);

        }

        [Fact]
        public void Update_User()
        {
            var userMangement = new UserManagement();
            userMangement.AddUser(new User("Sudhanshu", "Sharma"));
            var firstUser = userMangement.AllUsers.First();
            firstUser.Phone = "88585858585";
            userMangement.UpdatePhone(firstUser);
            var savedUser = Assert.Single(userMangement.AllUsers);
            Assert.NotNull(savedUser);
            Assert.Equal("88585858585", savedUser.Phone);
        }
    }
}