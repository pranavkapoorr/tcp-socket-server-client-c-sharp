using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace tcp_server{
    public class Server {
        private Socket server;

        public Server(String serverIp, int serverPort){
            try{
                this.server = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream, ProtocolType.Tcp);
                this.server.Bind(new IPEndPoint(IPAddress.Parse(serverIp),serverPort));
            } catch (SocketException e){
                Console.WriteLine("Unable to bind server: {0}",e);
                return;
            }
            this.server.Listen(20);
            Console.WriteLine("Waiting for connections!");
            acceptConnections();
            
        }

        public void acceptConnections(){
            while(true){
                Socket newClient = server.Accept();
                IPEndPoint clientDetails = (IPEndPoint)newClient.RemoteEndPoint;
                Console.WriteLine("new connection from "+clientDetails.Address+":"+clientDetails.Port);
                startReceiveSend(newClient,clientDetails);
            }
        }
        public void sendData(Socket client, NetworkStream networkStream){
            while(true){
                String input = Console.ReadLine();
                if (input == "exit"){
                    break;
                }
                networkStream.Write(Encoding.ASCII.GetBytes(input), 0, input.Length);
                networkStream.Flush();
            }
            endConnection();
        }

        public void receiveData(Socket client,IPEndPoint clientDetails, NetworkStream networkStream){
            while(true){
                byte[] data = new byte[1024];
                int recv = networkStream.Read(data, 0, data.Length);
                String stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(clientDetails.Address +":"+clientDetails.Port+ "= "+ stringData);
            }
        }

        public void endConnection(){
            try{
                //this.server.Shutdown();
            }catch(SocketException){
                Console.WriteLine("Unable to disconnect to server");
                return;
            }
             System.Environment.Exit(1);
        }

        public void startReceiveSend(Socket client,IPEndPoint clientDetails){
            NetworkStream networkStream = new NetworkStream(client);
            new Thread(()=>receiveData(client,clientDetails,networkStream)).Start();
            new Thread(()=>sendData(client,networkStream)).Start();
        }
    }
    public class TcpServerRunner{
        public static void Main(){
            new Server("0.0.0.0",2570);
        }
    }
}
