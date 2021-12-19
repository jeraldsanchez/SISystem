using SISystem.Data;
using SISystem.Models;
using SISystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class FunctionsRepository : IFunctionsRepository
    {
        #region <-- Functions -->
        public string CreatePin(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int randomInt = Between(0, 9);
                builder.Append(randomInt);
            }

            return builder.ToString();
        }

        public string CreateSalt(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int randomInt = Between(0, 9);
                builder.Append(randomInt);
            }
            string hex = EncryptSHA256(builder.ToString());
            return hex;
        }

        public string CreateSaltHashPassword(string password, string salted)
        {
            string hashedPassword = password + salted;

            return EncryptSHA256(hashedPassword);
        }

        public int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(randomNumber);

                double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

                // We are using Math.Max, and substracting 0.00000000001, 
                // to ensure "multiplier" will always be between 0.0 and .99999999999
                // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
                double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

                // We need to add one to the range, to allow for the rounding done with Math.Floor
                int range = maximumValue - minimumValue + 1;

                double randomValueInRange = Math.Floor(multiplier * range);

                return (int)(minimumValue + randomValueInRange);
            }
        }

        public string EncryptSHA256(string input)
        {
            string output = "";

            using (System.Security.Cryptography.SHA256 SHA1Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] data = SHA1Hash.ComputeHash(Encoding.UTF8.GetBytes(input.ToString()));
                StringBuilder sBuilder = new StringBuilder();
                int i = 0;
                for (i = 0; i <= data.Length - 1; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                output = sBuilder.ToString();
            }

            output = System.Web.HttpUtility.UrlEncode(output);

            return output;
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        public List<string> GetRoles(ConfigurationRoleList roles)
        {
            List<string> role = new List<string>();
            if (roles.ConfigurationRole != null) { 
            if (roles.ConfigurationRole.Accounting == true) { role.Add("AOX01"); }
            if (roles.ConfigurationRole.Warehouse == true) { role.Add("WB01"); }
            if (roles.ConfigurationRole.Store == true) { role.Add("SX01"); }
            if (roles.ConfigurationRole.BackOffice == true) { role.Add("BX01"); }
            if (roles.ConfigurationRole.Admin == true) { role.Add("AX01"); }
            }
            return role;
        }
        #endregion
    }
}
