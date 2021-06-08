using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SalesTaxApp
{
    public static class Validations
    {
        private static string[] allowedCharacters = new[] { "N", "Y" };

        public static bool RunInputValidationForYesNo(string input)
        {
            input = input.Trim().ToUpper();

            if (!allowedCharacters.Contains(input))
            {
                DisplayInputErrorMessage();
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool RunInputValidationForNo(string input)
        {
            input = input.Trim().ToUpper();

            if (!allowedCharacters[0].Contains(input))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool RunFilePathValidation(string filePath)
        {
            filePath.Replace(@"\\", @"\");
            var match = Regex.Match(filePath, @"^([A-Za-z]:+\\\\?([^\\/]*[\\/])*)([^\\/]+).(txt)$");

            if (!match.Success)
            {
                DisplayInputErrorMessage();
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void DisplayInputErrorMessage()
        {
            Console.WriteLine("\nError: You have entered invalid input, please enter valid input as indicated in the text instructions.");
        }

    }
}
