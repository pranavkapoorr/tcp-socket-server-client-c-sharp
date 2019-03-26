using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace tcp_client{
    public class Client {
        private TcpClient client;
        private NetworkStream networkStream;

        public Client(String hostIp, int hostPort){
            try{
                this.client = new TcpClient(hostIp, hostPort);
            } catch (SocketException){
                Console.WriteLine("Unable to connect to server");
                return;
            }
            this.networkStream = client.GetStream();
            startReceiveSend();
        }

        public void sendData(){
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

        public void receiveData(){
            while(true){
                byte[] data = new byte[1024];
                int recv = networkStream.Read(data, 0, data.Length);
                String stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("server: " + stringData);
            }
        }

        public void endConnection(){
            try{
                this.client.Close();
            }catch(SocketException){
                Console.WriteLine("Unable to disconnect to server");
                return;
            }
             System.Environment.Exit(1);
        }

        public void startReceiveSend(){
            new Thread(receiveData).Start();
            new Thread(sendData).Start();
        }
    }
    public class TcpClientRunner{
        public static void Main(){
            new Client("192.168.0.69",2570);
        }
    }
}
