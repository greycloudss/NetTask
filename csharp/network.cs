using System;
using System.Net;
using System.Net.Sockets;

public class Network {
	private int timer, amount;
    private Socket? socket;
    
    IPAddress? address;
    public Network(int timer, int amount) {
        this.timer = timer;
        this.amount = amount;

        address = new IPAddress(new byte[] { 127, 0, 0, 1 });
        Start();
    }

    public void Start() {
        
    
    }
}
