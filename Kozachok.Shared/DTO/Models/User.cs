using Kozachok.Shared.DTO.Common;
using Kozachok.Utils.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models
{
    public class User : Entity
    {
        // -> Empty contructor for EF
        public User()
        {

        }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password.Encrypt();
            CreatedDateUTC = DateTime.UtcNow;
        }

        public virtual string Name { get; private set; }
        public virtual string Email { get; private set; }
        public virtual string Password { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }

        public void UpdateInfo(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public void ChangePassword(string password) => Password = password.Encrypt();

        public bool CheckPassword(string password) => password.Encrypt() == Password;
    }
}
