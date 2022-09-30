namespace Bakery.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        public User(string login, string password) : this(login, password, null, null) { }

        public User(string login, string password, string? name, int? roleId)
        {
            Login = login;
            Password = password;
            Name = name;
            RoleId = roleId;
        }
    }
}
