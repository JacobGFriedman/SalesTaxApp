using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SalesTaxApp
{
    class SalesTaxConsole
    {
        const double importTax = 0.05;
        const double salesTax = 0.10;

        readonly OpertingModes currentMode;
        List<string> taxExemptItems = new List<string> { "Book", "Chocolate", "chocolates", "pills" };

        private double totalSalesTax = 0;
        private double totalSale = 0;

        public SalesTaxConsole(OpertingModes opertingMode)
        {
            currentMode = opertingMode;

            if (opertingMode == OpertingModes.Manual)
            {
                RunManually();
            }

            if (opertingMode == OpertingModes.Automatic)
            {
                var inputFilePath = GetInputFilePath();
                RunAutomatic(inputFilePath);
            }

            ConcludeSale();
        }

        private void RunAutomatic(string inputFilePath)
        {
            List<string> importedItems = File.ReadAllLines(inputFilePath).ToList();
            List<string> itemsToBeFormatted = new List<string>();
            List<string> formattedReciept = new List<string>();
            double modifiedItem;

            foreach(string item in importedItems)
            {
                if (itemsToBeFormatted.Contains(item))
                {
                    double.TryParse(item.Substring(0, 1), out modifiedItem);
                    modifiedItem++;
                    var index = itemsToBeFormatted.IndexOf(item);
                    itemsToBeFormatted.Remove(item);
                    var itemToAdd = modifiedItem + " " + item.Substring(1);
                    itemsToBeFormatted.Insert(index, itemToAdd);
                }
                else
                {
                    itemsToBeFormatted.Add(item);
                }
            }

            formattedReciept = ProcessItems(itemsToBeFormatted);

            foreach (string formattedItem in formattedReciept)
            {
                Console.WriteLine(formattedItem);
            }

            AddTotals();
        }

        private void RunManually()
        {
            List<string> itemsToBeFormatted = new List<string>();
            List<string> formattedReciept = new List<string>();
            bool moreItems = true;
            double modifiedItem;

            while (moreItems)
            {
                Console.WriteLine("\nPlease enter the item to be added in the following form: 1 Book at 12.49 (Number of Items, Item type, Price) \nor N when you are complete : ");

                var item = Console.ReadLine();

                if (!Validations.RunInputValidationForNo(item))
                {
                    if (itemsToBeFormatted.Contains(item))
                    {
                        double.TryParse(item.Substring(0, 1), out modifiedItem);
                        modifiedItem++;
                        var index = itemsToBeFormatted.IndexOf(item);
                        itemsToBeFormatted.Remove(item);
                        item = modifiedItem + " " + item.Substring(1);
                        itemsToBeFormatted.Insert(index, item);
                    }
                    else
                    {
                        itemsToBeFormatted.Add(item);
                    }
                }
                else
                {
                    moreItems = false;
                }
            }

            formattedReciept = ProcessItems(itemsToBeFormatted);

            foreach (string formattedItem in formattedReciept)
            {
                Console.WriteLine(formattedItem);
            }

            AddTotals();
        }

        private void AddTotals()
        {
            Console.WriteLine($" Sales Tax: " + string.Format("{0:0.00}", totalSalesTax));
            Console.WriteLine($" Total: {totalSale}");
        }

        private List<string> ProcessItems(List<string> items)
        {
            List<string> formmattedReciept = new List<string>();

            foreach (string item in items)
            {
                bool imported = false;
                bool taxExempt = false;
                double basePrice = 0;
                double numberOfUnits = 0;
                List<string> recieptDetails = new List<string>();

                if (item.Contains("Imported"))
                {
                    imported = true;
                }

                string[] itemParameters = item.Split(" ", StringSplitOptions.None);

                foreach(string parameter in itemParameters)
                {
                    if (taxExemptItems.Contains(parameter))
                    {
                        taxExempt = true;
                    }
                }

                double.TryParse(itemParameters[0], out numberOfUnits);

                foreach (string parameter in itemParameters)
                {
                    var isBasePrice = Regex.Match(parameter, @"\d+.\d+");

                    if (isBasePrice.Success)
                    {
                        double.TryParse(parameter, out basePrice);
                        continue;
                    }

                    if (parameter != "at" && parameter != string.Empty)
                    {
                        recieptDetails.Add(parameter);
                    }
                }

                var priceWithTax = CalculatePriceWithTax(imported, taxExempt, numberOfUnits, basePrice);
                formmattedReciept.Add(FormatForRecieptOutput(priceWithTax, numberOfUnits, recieptDetails));
            }

            return formmattedReciept;
        }

        private string FormatForRecieptOutput(double priceWithTax, double numberOfUnits, List<string> recieptDetails)
        {
            string formattedItem = string.Empty;

            foreach(string detail in recieptDetails)
            {
                if(!detail.Contains(numberOfUnits.ToString()))
                {
                    formattedItem += " " + detail;
                }
            }

            formattedItem += ": " + string.Format("{0:0.00}", priceWithTax);

            if(numberOfUnits > 1)
            {
                var pricePerUnit = priceWithTax / numberOfUnits;
                formattedItem += $" ({numberOfUnits} @ {pricePerUnit})";
            }

            return formattedItem;
        }

        private double CalculatePriceWithTax(bool imported, bool taxExempt, double numberOfUnits, double basePrice)
        {
            double taxRate = 0;

            if(imported)
            {
                taxRate += 0.05;
            }

            if (!taxExempt)
            {
                taxRate += 0.10;
            }

            if (taxRate > 0)
            {
                var roundedTax = Math.Round(((basePrice * numberOfUnits) * taxRate), 2, MidpointRounding.AwayFromZero);

                if(roundedTax % (0.5) != 0)
                {
                    double correction;

                    if (numberOfUnits < 2)
                    {
                        correction = Math.Ceiling(roundedTax/.05);
                        roundedTax = .05 * correction;
                    }
                    else
                    {
                        correction = Math.Ceiling((roundedTax/numberOfUnits)/.05);
                        roundedTax = (.05 * correction) * numberOfUnits;
                    }

                }

                totalSalesTax += roundedTax;
                totalSale += (basePrice * numberOfUnits) + roundedTax;

                return (basePrice * numberOfUnits) + roundedTax;
            }
            {
                totalSale += (basePrice * numberOfUnits);
                return (basePrice * numberOfUnits);
            }
            
        }

        private string GetInputFilePath()
        {
            Console.WriteLine("\nPlease enter the file path of the line delimited text input file in the following form C:\\Folder\\File.txt: ");

            var filePath = Console.ReadLine();

            if (!Validations.RunFilePathValidation(filePath))
            {
                GetInputFilePath();
            }

            return filePath;
        }

        private void ConcludeSale()
        {
            Console.WriteLine("\nThis sale has been completed please press any key to exit. Rerun the program for a new sale.");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
}
