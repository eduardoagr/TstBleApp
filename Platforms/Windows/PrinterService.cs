using System.Drawing;


using TstBleApp.Interfaces;

using PrintDocument = System.Drawing.Printing.PrintDocument;

namespace TstBleApp.Platforms.Windows {

    public class PrinterService : IPrinterService {

        public void PrintText(string text, string printerName) {

            PrintDocument pd = new();

            pd.PrinterSettings.PrinterName = printerName;

            pd.PrintPage += (sender, args) => {
                args.Graphics?.DrawString(text, new System.Drawing.Font("Arial", 14), Brushes.Black, 10, 10);
            };

            pd.Print();


        }
    }
}
