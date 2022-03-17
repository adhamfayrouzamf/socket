using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace socket
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            string fileName = Console.ReadLine();
            Console.WriteLine("Adham Mohamed Fayrouz Ali\nIS\nSection 1");
            byte[] fileBuff = File.ReadAllBytes(fileName);
            byte[] stringBuff = Encoding.ASCII.GetBytes(fileName);
            byte[] sizeBuff = BitConverter.GetBytes(fileName.Length);
            byte[] dataBuff = new byte[4 + fileName.Length + fileBuff.Length];
            sizeBuff.CopyTo(dataBuff, 0);
            stringBuff.CopyTo(dataBuff, 4);
            fileBuff.CopyTo(dataBuff, 4 + fileName.Length);
            SocketServer socket = new SocketServer(IPAddress.Parse("127.0.0.1"), 8000);
            socket.onConnection((Socket sock)=>
            {
                Console.WriteLine("Sending File: {0}",fileName);
                sock.Send(dataBuff, 0, dataBuff.Length, SocketFlags.None);
                Console.WriteLine("File Sent");
            });
            socket.SetupSocket(100);
            string text = "";
            while (text.ToLower() != "end")
            {
                text = Console.ReadLine();
            }
            socket.Uninstall();
            //fs.Close();
            
            
        }
        
    }
}
