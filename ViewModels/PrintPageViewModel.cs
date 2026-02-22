using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TstBleApp.Interfaces;

namespace TstBleApp.ViewModels;

public partial class PrintPageViewModel(IPrinterService printer) : ObservableObject {

    [RelayCommand]
    void Print() {
        if(printer is null)
            return; // o mostrar mensaje

        printer.PrintText("Hola desde MAUI Windows!");
    }
}