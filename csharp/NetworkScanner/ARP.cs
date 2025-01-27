using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetTask.NetworkScanner {
    internal class ARP {
        PhysicalAddress? selfMac;
        Socket rawSock;
        IPAddress self, target;
        int timer;

        byte[] handshakeResponse;

        public ARP(IPAddress self, IPAddress target, int timer) {
            selfMac = getMac();
            this.target = target;
            this.self = self;
            this.timer = timer;
            Start();
        }

        private void Start() {

            rawSock = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Raw);

            ARP_Handshake();
        }

        private byte[] buildPacket() {
            byte[] packet = new byte[42];


            Array.Copy(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0, packet, 0, 6);
            Array.Copy(selfMac.GetAddressBytes(), 0, packet, 6, 6);
            Array.Copy(new byte[] { 0x08, 0x06 }, 0, packet, 12, 2);


            Array.Copy(new byte[] { 0x00, 0x01 }, 0, packet, 14, 2);
            Array.Copy(new byte[] { 0x08, 0x00 }, 0, packet, 16, 2);

            packet[18] = 6;
            packet[19] = 4;

            Array.Copy(new byte[] { 0x00, 0x01 }, 0, packet, 20, 2);
            Array.Copy(selfMac.GetAddressBytes(), 0, packet, 22, 6);
            Array.Copy(self.GetAddressBytes(), 0, packet, 28, 4);
            Array.Copy(new byte[6], 0, packet, 32, 6);
            Array.Copy(target.GetAddressBytes(), 0, packet, 38, 4);

            return packet;
        }

        private async Task ARP_Handshake() {
            
            try {
                rawSock.Connect(target, 0);

                rawSock.Send(buildPacket());

                await Task.Delay(timer * 1000);

                rawSock.Receive(handshakeResponse);

                foreach (var packet in handshakeResponse) {
                    Console.WriteLine(packet);
                }
                
            } catch {
                throw new Exception("ARP handshake failed.");
            } finally { 
                rawSock.Close();
            }
        }

        private PhysicalAddress? getMac() {
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces()) {
                if (item.OperationalStatus == OperationalStatus.Up)
                    return item.GetPhysicalAddress();
            }

            return null;
        }
    }
}
