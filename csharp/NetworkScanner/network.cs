using System;
using System.Net;
using System.Net.Sockets;

public class Network {
    private int timer, amount;
    private Socket? socket;

    private IPAddress? address, subNetMask;
    public Network(int timer, int amount) {
        this.timer = timer;
        this.amount = amount;

        address = new IPAddress(new byte[] { 127, 0, 0, 1 });
        Start();
    }

    private byte[]? returnClass() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        byte[]? ipBytes = null;
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                ipBytes = ip.GetAddressBytes();
                address = new IPAddress(ipBytes);
                break;
            }
        }

        Console.Write($"The current IP is {address} of class: ");


        // Determine the IP class and subnet mask
        switch (ipBytes[0]) {
            case >= 1 and < 128:
                ipBytes = new byte[] { 255, 0, 0, 0 };
                Console.WriteLine("A");
                break;

            case >= 128 and < 192:
                ipBytes = new byte[] { 255, 255, 0, 0 };
                Console.WriteLine("B");
                break;

            case >= 192 and < 224:
                ipBytes = new byte[] { 255, 255, 255, 0 };
                Console.WriteLine("C");
                break;

            case >= 224 and < 240:
                Console.WriteLine("D");
                return null;

            case >= 240 and <= 255:
                Console.WriteLine("E");
                return null;

            default:
                Console.WriteLine("UNKNOWN");
                return null;
        }

        return ipBytes;
    }

    public void Start() {
        byte[]? subnetMaskBytes = returnClass();
        if (subnetMaskBytes != null) {
            IPAddress subnetMask = new IPAddress(subnetMaskBytes);
            Console.WriteLine($"Subnet Mask: {subnetMask}");
        }
    }
}