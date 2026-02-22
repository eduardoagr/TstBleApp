using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TstBleApp.ViewModels;

public partial class BluetoothPageViewModel : ObservableObject {

    private readonly IBluetoothLE _ble;
    private readonly IAdapter _adapter;

    public ObservableCollection<IDevice> Devices { get; set; } = [];

    [ObservableProperty]
    public partial bool IsScanning { get; set; }

    private IDevice? _connectedDevice;

    [ObservableProperty]
    public partial string Status { get; set; } = "Ready";

    public BluetoothPageViewModel() {
        _ble = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;

        _adapter.DeviceDiscovered += (s, e) => {
            Debug.WriteLine($"FOUND: {e.Device.Name} - {e.Device.Id}");

            MainThread.BeginInvokeOnMainThread(() => {
                if(!Devices.Any(d => d.Id == e.Device.Id))
                    Devices.Add(e.Device);
            });
        };
    }

    #region 🔍 Scan

    [RelayCommand]
    private async Task StartScanAsync() {
        if(!_ble.IsOn) {
            Status = "Bluetooth is OFF";
            return;
        }

        Devices.Clear();
        IsScanning = true;
        Status = "Scanning...";

        try {
            await _adapter.StartScanningForDevicesAsync();
        } catch(Exception ex) {
            Status = $"Error: {ex.Message}";
        } finally {
            IsScanning = false;
            Status = "Finished";
        }
    }

    #endregion

    #region 🔵 Connect

    [RelayCommand]
    private async Task ConnectAsync(IDevice device) {
        if(device == null)
            return;

        try {
            Status = "Connecting...";

            await _adapter.ConnectToDeviceAsync(device);

            _connectedDevice = device;

            Status = $"Connected: {device.Name}";

        } catch(Exception ex) {
            Status = $"Connect failed: {ex.Message}";
        }
    }

    #endregion

    #region 🖨️ Print

    [RelayCommand]
    private async Task PrintAsync() {
        if(_connectedDevice == null) {
            Status = "Not connected";
            return;
        }

        Status = "Printing...";

        var text =
            "<C>HELLO EDUARDO\n" +
            "<B>MAUI WINDOWS</B>\n";

        var success = await Printer.PrintTextAsync(
            _connectedDevice,
            text);

        Status = success ? "Printed" : "Print failed";
    }

    #endregion

    [RelayCommand]
    private async Task StopScanAsync() {
        if(IsScanning) {
            await _adapter.StopScanningForDevicesAsync();
            IsScanning = false;
            Status = "Stopped";
        }
    }

}