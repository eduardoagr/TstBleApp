using TstBleApp.ViewModels;

namespace TstBleApp.Views;

public partial class PrintPage : ContentPage {

    public PrintPage(PrintPageViewModel vm) {
        InitializeComponent();

        BindingContext = vm;
    }
}