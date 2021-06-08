using System;

namespace SalesTaxApp
{
    class Program
    {
        private static SalesTaxConsole app;
        private static bool properInput;

        static void Main(string[] args)
        {
            while(!properInput)
            {
                var input = ObtainOperatingMode();

                properInput = Validations.RunInputValidationForYesNo(input);

                if (properInput)
                {
                    StartApplication(input);
                }
            }
        }

        private static void StartApplication(string input)
        {
            if (input.Trim().ToUpper() == "Y")
            {
                app = new SalesTaxConsole(OpertingModes.Manual);
            }
            else if (input.Trim().ToUpper() == "N")
            {
                app = new SalesTaxConsole(OpertingModes.Automatic);
            }
        }

        private static string ObtainOperatingMode()
        {
            Console.WriteLine("\nWould you like to enter the items manually or use the automatic mode, with a line delimited text file," +
                              "\nto retreive your receipt? (Answer Y or N only): ");

            return Console.ReadLine();
        }
    }
}
