using TstBleApp.ViewModels;

namespace TstBleApp.Views;

public partial class BluetoothPage : ContentPage {
    public BluetoothPage(BluetoothPageViewModel bluetoothPageViewModel) {
        InitializeComponent();

        BindingContext = bluetoothPageViewModel;
    }
}