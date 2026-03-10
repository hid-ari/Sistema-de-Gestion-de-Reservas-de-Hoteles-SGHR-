using System.Text.RegularExpressions;

namespace SGHR.Data.Helpers
{
    public static class PasswordValidator
    {
        public static bool EsPasswordFuerte(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, "[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, "[0-9]"))
                return false;

            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                return false;

            return true;
        }
    }
}