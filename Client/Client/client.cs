﻿using System;
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
        static LinkedList<String> incommingMessages = new LinkedList<string>();

        static void serverReceiveThread(Object obj)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] receiveBuffer = new byte[8192];

            Socket s = obj as Socket;

            while (true)
            {
                try
                {
                    int reciever = s.Receive(receiveBuffer);
                    s.Receive(receiveBuffer);
                    if (reciever > 0)
                    {
                        String clientMsg = encoder.GetString(receiveBuffer, 0, reciever);
                        //Console.Clear(); // Clears the screen when so its not crammed with every message
                        Console.WriteLine(clientMsg); // Prints replies from server
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex); // Prints any errors
                }
            }
        }

        static void Main(string[] args)
        {

            string ipAdress = "127.0.0.1"; // Server IP 165.227.225.88
            int port = 8221;

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse(ipAdress), port);

            bool connected = false;

            while (connected == false)
            {
                Console.WriteLine("Looking for server: " + ipLocal);
                try
                {
                    s.Connect(ipLocal);
                    Console.Clear();
                    Console.WriteLine("Connected To Server\n\nType help for assistance");

                    connected = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }

            int ID = 0;


            var myThread = new Thread(serverReceiveThread);
            myThread.Start(s);

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = new byte[4096];

            while (true)
            {
                String ClientText = Console.ReadLine();
                ID++;
                buffer = encoder.GetBytes(ClientText);

                try
                {
                    // Writes messages to server
                    Console.WriteLine("Writing to server: " + ClientText);
                    int bytesSent = s.Send(buffer);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}