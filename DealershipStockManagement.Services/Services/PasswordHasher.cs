using System.Security.Cryptography;
using System.Text;
namespace DealershipStockManagement.Application.Services
{
    public static class PasswordHasher
    {
        // Hash a password and generate a unique salt
        public static (byte[] Hash, byte[] Salt) HashPassword(string password)
        {
            using var hmac = new HMACSHA512(); // generates a random key (salt)
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        // Verify a password against a stored hash and salt
        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (computedHash.Length != storedHash.Length)
                return false;

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                    return false;
            }

            return true;
        }
    }
}
