using System;
using System.Net;
using System.Net.Sockets;

public class Port {
    private int amount;
    private IPAddress pAddress;

    private int maxPort = 65535;
    List<int> openPorts = new();

    //i highly regret writing this in c#

    public Port(IPAddress pAddress, int amount) {
        this.amount = amount;
        this.pAddress = pAddress;

        Task.Run(async () => await StartAsync()).Wait();

        foreach (int port in openPorts) {
        
            Console.Write(port.ToString() + " ");
        }
    }

    private async Task StartAsync() {
        List<Task> tasks = new();
        int tmpMax = amount == -1 ? maxPort : Math.Min(maxPort, amount);
        for (int i = 0; i < tmpMax; i++) {
            tasks.Add(ScanPortAsync(i));
        }

        await Task.WhenAll(tasks);
    }

    private async Task ScanPortAsync(int port) {
        try {
            using TcpClient client = new();
            var cts = new CancellationTokenSource(500);
            await client.ConnectAsync(pAddress, port).WaitAsync(cts.Token);

            lock (openPorts) {
                openPorts.Add(port);
            }
            Console.Write($"{port} ");
        } catch (Exception) { }
    }
}
