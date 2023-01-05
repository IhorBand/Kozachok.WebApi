using Kozachok.Shared.DTO.Common;
using Kozachok.Utils.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class User : Entity
    {
        // -> Empty contructor for EF
        public User()
        {

        }

        public User(string name, string email, string password, bool isActive)
        {
            Name = name;
            Email = email;
            Password = password.Encrypt();
            CreatedDateUTC = DateTime.UtcNow;
            IsActive = isActive;
        }

        public virtual string Name { get; private set; }
        public virtual string Email { get; private set; }
        public virtual string Password { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual Guid? ThumbnailImageFileId { get; private set; }
        public virtual DateTime CreatedDateUTC { get; private set; }

        public void UpdateInfo(string name)
        {
            Name = name;
        }

        public void SetThumbnailImageFileId(Guid fileId) => ThumbnailImageFileId = fileId;

        public void ChangeEmail(string email) => Email = email;

        public void ChangePassword(string password) => Password = password.Encrypt();

        public bool CheckPassword(string password) => password.Encrypt() == Password;

        public void Activate() => IsActive = true;
    }
}
