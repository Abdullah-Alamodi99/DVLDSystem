using Microsoft.AspNetCore.Identity;

namespace DVLDSystem.Custom
{
    public class CustomIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "يجب أن تحتوي كلمات المرور على حرف واحد غير أبجدي رقمي على الأقل."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"يجب أن تكون كلمة المرور مكونة من {length} أحرف على الأقل."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل."
            };
        }
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = "كلمة المرور غير صحيحة"
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"أسم المستخدم {userName} موجود مسبقا"
            };
        }
    }
}
