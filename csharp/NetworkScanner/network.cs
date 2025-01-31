using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetTask.NetworkScanner {
    internal class Network {
        private readonly ConcurrentBag<(IPAddress, string)> results = new();
        private readonly HashSet<string> uniqueMacs = new();
        private static readonly SemaphoreSlim semaphore = new(100);

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        static extern int SendARP(uint destIp, uint srcIp, byte[] macAddr, ref uint macAddrLen);

        public Network() {
            Task.Run(Start).Wait();
        }

        public List<(IPAddress, string)> ReturnAddresses() {
            return results.ToList();
        }

        private List<IPAddress[]> GetLocalIPs() {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ip.IPv4Mask != null)
                .Select(ip => new IPAddress[] { ip.Address, ip.IPv4Mask! })
                .ToList();
        }

        private List<IPAddress>? GetSubnetIPs(IPAddress[] ipInfo) {
            if (ipInfo.Length < 2) return null;

            uint networkAddress = BitConverter.ToUInt32(ipInfo[0].GetAddressBytes().Reverse().ToArray(), 0) &
                                  BitConverter.ToUInt32(ipInfo[1].GetAddressBytes().Reverse().ToArray(), 0);

            uint broadcastAddress = networkAddress | (~BitConverter.ToUInt32(ipInfo[1].GetAddressBytes().Reverse().ToArray(), 0) & 0xFFFFFFFF);
            uint totalIPs = broadcastAddress - networkAddress - 1;

            if (totalIPs < 1) return null;

            return Enumerable.Range(1, (int)totalIPs)
                .Select(i => new IPAddress(BitConverter.GetBytes(networkAddress + (uint)i).Reverse().ToArray()))
                .ToList();
        }

        private async Task<string?> GetMacAddress(IPAddress ipAddress) {
            await semaphore.WaitAsync();
            try {
                byte[] macAddr = new byte[6];
                uint macAddrLen = (uint)macAddr.Length;
                uint destIp = BitConverter.ToUInt32(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);

                if (SendARP(destIp, 0, macAddr, ref macAddrLen) != 0) return null;
                if (macAddr.All(b => b == 0x00) || macAddr.All(b => b == 0xFF)) return null;

                return FormatMacAddress(macAddr);
            } finally {
                semaphore.Release();
            }
        }

        private static string FormatMacAddress(byte[] macBytes) {
            return string.Join(":", macBytes.Select(b => b.ToString("X2")));
        }

        private async Task Start() {
            List<IPAddress[]> localNetworks = GetLocalIPs();
            List<IPAddress> allIPs = localNetworks.SelectMany(GetSubnetIPs).Where(ip => ip != null).ToList()!;

            await Parallel.ForEachAsync(allIPs, new ParallelOptions { MaxDegreeOfParallelism = 200 }, async (ip, _) => {
                string? mac = await GetMacAddress(ip);
                if (mac != null) {
                    lock (uniqueMacs) {
                        if (!uniqueMacs.Contains(mac)) {
                            uniqueMacs.Add(mac);
                            results.Add((ip, mac));
                            Console.WriteLine($"{ip} {mac}");
                        }
                    }
                }
            });
        }
    }
}
