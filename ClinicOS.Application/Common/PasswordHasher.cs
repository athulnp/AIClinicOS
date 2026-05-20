using System.Security.Cryptography;
using System.Text;

namespace ClinicOS.Application.Common;

/// <summary>
/// BCrypt password hashing with automatic upgrade from legacy SHA-256 hashes on login.
/// </summary>
public static class PasswordHasher
{
    private const int BcryptWorkFactor = 12;

    public static string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor);

    /// <summary>
    /// Verifies a password. Sets <paramref name="needsRehash"/> when a legacy SHA-256 hash matched
    /// so the caller can persist a new BCrypt hash.
    /// </summary>
    public static bool Verify(string password, string hash, out bool needsRehash)
    {
        needsRehash = false;

        if (IsBcryptHash(hash))
            return BCrypt.Net.BCrypt.Verify(password, hash);

        if (!VerifyLegacySha256(password, hash))
            return false;

        needsRehash = true;
        return true;
    }

    public static bool Verify(string password, string hash) =>
        Verify(password, hash, out _);

    private static bool IsBcryptHash(string hash) =>
        hash.StartsWith("$2a$", StringComparison.Ordinal)
        || hash.StartsWith("$2b$", StringComparison.Ordinal)
        || hash.StartsWith("$2y$", StringComparison.Ordinal);

    private static bool VerifyLegacySha256(string password, string hash)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var computed = Convert.ToBase64String(sha.ComputeHash(bytes));
        return computed == hash;
    }
}
