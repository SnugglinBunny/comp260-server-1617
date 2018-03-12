using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class client
    {
        static void Main(string[] args)
        {
            string ServerIP = "127.0.0.1";
            int Port = 8221;
            String UserID = null;

            Console.WriteLine("Please enter a username:");
            UserID = Console.ReadLine();
            Console.WriteLine("Welcome to my MUD " + UserID);

            Console.WriteLine("Would you like to connect to this server?\n" + ServerIP + ":" + Port);
            String newServer = Console.ReadLine();
            if (newServer == "yes")
            {
                Console.WriteLine("Connecting to server " + ServerIP + ":" + Port + "...");
            }
            else
            {
                Console.WriteLine("Firstly, what IP would you like to connect to?");
                ServerIP = Console.ReadLine();
                Console.WriteLine("And what Port?");
                Port = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Thank you, the new IP and Port are: " + ServerIP + ":" + Port + "\n Trying Server, please wait...");
            }

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint Server = new IPEndPoint(IPAddress.Parse(ServerIP), Port);

            bool connected = false;

            while (connected == false)
            {
                try
                {
                    s.Connect(Server);
                    connected = true;
                    Console.WriteLine("Connected to Server!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot find server, retrying...");
                    Thread.Sleep(1000);
                }
            }

            int ID = 0;

            while (true)
            {
                //Console.Clear();
                Console.Write("\n> ");
                String ClientText = Console.ReadLine();
                String Msg = ID.ToString() + ClientText; // " testing, testing, 1,2,3";
                ID++;
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(ClientText);

                try
                {
                    Console.WriteLine("Writing to server: " + ClientText);
                    int bytesSent = s.Send(buffer);

                    buffer = new byte[4096];
                    int reciever = s.Receive(buffer);
                    if (reciever > 0)
                    {
                        Console.Clear();
                        String userCmd = encoder.GetString(buffer, 0, reciever);
                        Console.WriteLine(userCmd);
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);	
                }
                

                //Thread.Sleep(1000);
            }
        }
    }
}
