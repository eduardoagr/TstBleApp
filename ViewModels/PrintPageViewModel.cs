using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;

#if WINDOWS
using System.Management;
#endif

using TstBleApp.Interfaces;

namespace TstBleApp.ViewModels;

public partial class PrintPageViewModel : ObservableObject {
    private readonly IPrinterService _printerService;

    public ObservableCollection<string> DevicesUsb { get; } = [];

    [ObservableProperty]
    public partial string SelectedDevice { get; set; }

    public PrintPageViewModel(IPrinterService printerService) {
        _printerService = printerService;

#if WINDOWS
        LoadUsbDevices();
#endif
    }

    [RelayCommand]
    private void Print() {
        if(_printerService is null || string.IsNullOrWhiteSpace(SelectedDevice))
            return;

        _printerService.PrintText("Hola desde MAUI Windows!", SelectedDevice);
    }

#if WINDOWS
    private void LoadUsbDevices() {

        var devices = GetUsbDevices();

        DevicesUsb.Clear();
        foreach(var d in devices)
            DevicesUsb.Add(d);
    }

    private List<string> GetUsbDevices() {

        var devices = new List<string>();

        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBHub");

        foreach(var device in searcher.Get()) {
            var name = device["Name"]?.ToString();
            if(!string.IsNullOrWhiteSpace(name))
                devices.Add(name);
        }

        return devices;
    }
#endif
}