using Kozachok.Shared.DTO.Common;
using Kozachok.Utils.Cryptography;
using System;
using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class User : Entity
    {
        public virtual string Name { get; private set; }
        public virtual string Email { get; private set; }
        public virtual string Password { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual Guid? ThumbnailImageFileId { get; private set; }
        public virtual DateTime CreatedDateUtc { get; private set; }

        public virtual ICollection<RoomUser> RoomUsers { get; set; }
        public virtual File ThumbnailImageFile { get; set; }

        public void UpdateInfo(string name)
        {
            Name = name;
        }

        public void SetThumbnailImageFileId(Guid fileId) => ThumbnailImageFileId = fileId;

        public void ChangeEmail(string email) => Email = email;

        public void ChangePassword(string password) => Password = password.Encrypt();

        public bool CheckPassword(string password) => password.Encrypt() == Password;

        public void Activate() => IsActive = true;

        public static User Create(string name, string email, string password, bool isActive)
        {
            return new User
            {
                Name = name,
                Email = email,
                Password = password.Encrypt(),
                CreatedDateUtc = DateTime.UtcNow,
                IsActive = isActive,
            };
        }
    }
}
