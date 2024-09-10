namespace MobitekCRMV2.Business.Services
{
    using System;
    using System.Linq;

    public class PasswordService
    {
        private Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&*()-_=+?";

        public string GenerateRandomPassword()
        {
            int length = 10;
            string password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            while (!IsValidPassword(password))
            {
                password = new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return password;
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;


            if (password.ToLower() == password || password.ToUpper() == password)
                return false;

            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            bool hasSymbol = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpperCase = true;
                else if (char.IsLower(c))
                    hasLowerCase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else if (char.IsSymbol(c) || char.IsPunctuation(c))
                    hasSymbol = true;
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSymbol;
        }
    }

}
