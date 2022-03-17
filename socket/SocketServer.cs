using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace socket
{
    class SocketServer
    {
        private byte[] buffer = new byte[1024];
        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private int port = 8000;
        private List<Socket> clientSockets = new List<Socket>();
        private Socket socketIO = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
            );
        
        public SocketServer(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        public void SetupSocket(int backlog=5)
        {
            socketIO.Bind(new IPEndPoint(ip, port));
            socketIO.Listen(backlog);
            Console.WriteLine("Listening at IP: {0}, Port: {1}",ip.ToString(),port);
            socketIO.BeginAccept(AcceptCallBack, null);
        }
        private void AcceptCallBack(IAsyncResult Rs)
        {
            try
            {
                Socket socket = socketIO.EndAccept(Rs);
                clientSockets.Add(socket);
                Console.WriteLine("Client Accepted");
                if(connection!=null)
                    connection(socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallBack, socket);
                socketIO.BeginAccept(AcceptCallBack, null);
            }
            catch (Exception ex)
            {
                socketIO.BeginAccept(AcceptCallBack, null);
                Console.WriteLine(ex.Message);
            }
        }
        private void ReceiveCallBack(IAsyncResult Rs)
        {
            try
            {
                Socket socket = (Socket)Rs.AsyncState;
                string text = "";
                int received = socket.EndReceive(Rs);
                byte[] dataBuff = new byte[received];
                Array.Copy(buffer, dataBuff, received);
                text = Encoding.ASCII.GetString(dataBuff);
                if (text.ToLower() != "end")
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
                
                Console.WriteLine("text is " + text);
            }
            catch (Exception ex)
            {
                Disconnect((Socket)Rs.AsyncState);
                Console.WriteLine(ex.Message);
            }
        }
        private static void sendData(string data, Socket socket)
        {
            byte[] dataInBytes = Encoding.ASCII.GetBytes(data);
            socket.Send(dataInBytes,0,dataInBytes.Length,SocketFlags.None);
        }
        private void Disconnect(Socket socket)
        {
            //Socket socket = (Socket)Rs.AsyncState;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            }catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        public void Uninstall()
        {
            Disconnect(socketIO);
        }

        public delegate void sendFileOnConnection(Socket socket);
        private sendFileOnConnection connection;
        public void onConnection(sendFileOnConnection callback)
        {
            connection = callback;
        }
        

    }
}
