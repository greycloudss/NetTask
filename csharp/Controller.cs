using System.Net;

namespace NetTask {
    public class NetTask {
        private string[] args;
        private readonly string[] helpDesk = { "-h", "--help", "-port", "-net", "-t", "-amount" };
        private readonly string[] explanations = { "Lists commands", "Tries to scan ports of wanted address", "Tries to list all of the network MAC addresses",
        "Time before timeout", "Amount of scans that will be done" };


        private Network? network = null;
        private Port? port = null;

        private int timer = -1, amount = -1;



        //redundant
        private byte[]? ParseToByteArr() {
            if (args[0].Contains(',') || string.IsNullOrEmpty(args[0]))
                return null;
           
            string[] sBytes = args[0].Split('.');
            byte[] bytes = new byte[sBytes.Length];
            foreach (string s in sBytes)
                bytes.Append(byte.Parse(s));
            
            //no checking for port input yet

            return bytes;
        }

        private bool matchFlags() {
            string? prevFlag = null;

            foreach (string arg in args) {
               if (prevFlag == null) {
                    prevFlag = arg;
                    continue;
               }

                if (arg.Equals(prevFlag))
                    return false;

                switch (prevFlag) {
                    case "-t":
                        timer = int.Parse(arg);
                        break;
                    
                    case "-amount":
                        amount = int.Parse(arg);
                        break;

                    default:
                        continue;
                }


                prevFlag = arg;
            }

            return true;
        }


        private void parseFlags() {
            if (args.Contains("-h") || args.Contains("--help")) {
                seekHelp();
                return;
            }

            matchFlags();

            if (args.Contains("-net"))
                network = new(timer, amount);

            if (args.Contains("-port")) {
                Console.WriteLine("starting");
                port = new(IPAddress.Parse(args[0]), amount);
            }
            if (port != null && network != null)
                throw new Exception("only 1 of the opperations is allowed to be done at a time");

        }


        private void seekHelp() {
            Console.WriteLine("Help Desk");
            Console.WriteLine("Flags:");
            Console.WriteLine($"{helpDesk[0]}, {helpDesk[1]} - {explanations[0]}");

            for (int i = 2; i < args.Length; ++i)
                Console.WriteLine(helpDesk[i] + " - " + explanations[i]);

            return;
        }

        public NetTask(string[] args) {
            this.args = args;
            parseFlags();
        }

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("No arguments provided");
                return;
            }
            NetTask netTask = new NetTask(args);
        }
    }
}