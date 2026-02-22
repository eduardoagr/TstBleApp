using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

using System.Diagnostics;
using System.Text;

namespace TstBleApp;

public enum TextAlign {
    Left,
    Center,
    Right
}

public static class Printer {

    private static readonly Encoding PrinterEncoding =
        Encoding.GetEncoding(437);

    public static async Task<bool> PrintTextAsync(
      IDevice device,
      string text,
      TextAlign defaultAlignment = TextAlign.Left) {


        if(device.State != DeviceState.Connected)
            return false;

        var characteristic = await GetWritableCharacteristicAsync(device);

        if(characteristic == null)
            return false;

        var data = BuildEscPos(text, defaultAlignment);

        System.Diagnostics.Debug.WriteLine(characteristic.Properties);

        try {
            await characteristic.WriteAsync(data, CancellationToken.None);
            return true;
        } catch(Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }
    }

    private static async Task<ICharacteristic?> GetWritableCharacteristicAsync(IDevice device) {
        var services = await device.GetServicesAsync();

        foreach(var service in services) {

            Debug.WriteLine($"SERVICE: {service.Id}");

            foreach(var ch in await service.GetCharacteristicsAsync()) {
                Debug.WriteLine($"  CHAR: {ch.Id}  PROPS: {ch.Properties}");
            }

            var characteristics = await service.GetCharacteristicsAsync();

            var writeNoResp = characteristics.FirstOrDefault(c =>
                c.Properties.HasFlag(CharacteristicPropertyType.WriteWithoutResponse));

            if(writeNoResp != null)
                return writeNoResp;

            var write = characteristics.FirstOrDefault(c =>
                c.Properties.HasFlag(CharacteristicPropertyType.Write));

            if(write != null)
                return write;
        }

        return null;
    }

    private static byte[] BuildEscPos(
        string text,
        TextAlign defaultAlignment) {
        var bytes = new List<byte>();

        // Initialize printer
        bytes.AddRange([0x1B, 0x40]);

        foreach(var raw in text.Split('\n')) {
            if(string.IsNullOrWhiteSpace(raw)) {
                bytes.Add((byte)'\n');
                continue;
            }

            var line = raw.Trim();

            // Alignment
            if(line.StartsWith("<C>")) {
                bytes.AddRange([0x1B, 0x61, 0x01]);
                line = line[3..];
            } else if(line.StartsWith("<R>")) {
                bytes.AddRange([0x1B, 0x61, 0x02]);
                line = line[3..];
            } else if(line.StartsWith("<L>")) {
                bytes.AddRange([0x1B, 0x61, 0x00]);
                line = line[3..];
            } else {
                bytes.AddRange(defaultAlignment switch {
                    TextAlign.Center => [0x1B, 0x61, 0x01],
                    TextAlign.Right => [0x1B, 0x61, 0x02],
                    _ => [0x1B, 0x61, 0x00]
                });
            }

            // Bold
            bool bold = line.Contains("<B>");
            line = line.Replace("<B>", "").Replace("</B>", "");
            bytes.AddRange([0x1B, 0x45, (byte)(bold ? 1 : 0)]);

            // Underline
            bool underline = line.Contains("<U>");
            line = line.Replace("<U>", "").Replace("</U>", "");
            bytes.AddRange([0x1B, 0x2D, (byte)(underline ? 1 : 0)]);

            // Text
            bytes.AddRange(PrinterEncoding.GetBytes(line));
            bytes.Add((byte)'\n');
        }

        return bytes.ToArray();
    }
}