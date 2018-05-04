using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection 	=Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand 	=Mono.Data.Sqlite.SqliteCommand;
using sqliteDataReader	=Mono.Data.Sqlite.SqliteDataReader;
#endif

#if TARGET_WINDOWS
using System.Data.SQLite;
using sqliteConnection = System.Data.SQLite.SQLiteConnection;
using sqliteCommand = System.Data.SQLite.SQLiteCommand;
using sqliteDataReader = System.Data.SQLite.SQLiteDataReader;
#endif

namespace Server
{
    public class server
    {
        static LinkedList<String> inMessages = new LinkedList<string>();  // Creates a list for incoming messages
        static LinkedList<String> outMessages = new LinkedList<string>(); // Creates a list for outgoing messages

        static Dictionary<String, Socket> clientDictionary = new Dictionary<String, Socket>(); // Creates a list of sockets

        public static List<Player> PlayerList = new List<Player>(); // Creates a list of players
        static Dungeon dungeon = new Dungeon(); // Creates  a new instance of the dungeon

        class ReceiveThreadLaunchInfo
        {
            public ReceiveThreadLaunchInfo(int ID, Socket socket)
            {
                this.ID = ID;
                this.socket = socket;
            }

            public int ID;
            public Socket socket;

        }

        static void acceptClientThread(Object obj)
        // Thread for accepting new clients
        {
            Socket s = obj as Socket;
            int ID = 1;

            while (true)
            {
                var newClientSocket = s.Accept();

                var myThread = new Thread(clientReceiveThread);
                myThread.Start(new ReceiveThreadLaunchInfo(ID, newClientSocket));

                lock (clientDictionary)
                {

                    String clientName = "client" + ID;
                    clientDictionary.Add(clientName, newClientSocket);
                    Console.WriteLine(clientName + ": Connected");
                    var player = new Player
                    {
                        dungeonRef = dungeon,
                        playerName = "Player" + ID,
                        clientID = clientName
                    };
                    player.Init();
                    PlayerList.Add(player);

                    var dungeonResult = dungeon.DungeonInfo(player, true);

                    lock (outMessages)
                    {
                        outMessages.AddLast(clientName + ":" + dungeonResult);
                    }
                    ID++;
                }
            }
        }

        static Socket GetSocketFromName(String name)
        // Gets socket from username
        {
            lock (clientDictionary)
            {
                return clientDictionary[name];
            }
        }

        static String GetNameFromSocket(Socket s)
        // Gets username from socket
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> o in clientDictionary)
                {
                    if (o.Value == s)
                    {
                        return o.Key;
                    }
                }
            }
            return null;
        }

        static void clientReceiveThread(Object obj)
        // Thread for accepting new clients
        {
            ReceiveThreadLaunchInfo receiveInfo = obj as ReceiveThreadLaunchInfo;
            bool socketLost = false;

            while (socketLost == false)
            {
                byte[] buffer = new byte[4094];

                try
                {
                    int result = receiveInfo.socket.Receive(buffer);

                    if (result > 0)
                    {
                        ASCIIEncoding encoder = new ASCIIEncoding();

                        lock (inMessages)
                        {
                            inMessages.AddLast(receiveInfo.ID + ":" + encoder.GetString(buffer, 0, result));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    socketLost = true;
                }
            }
        }

        static void globalChatMsg(string message)
        // Controls global chat messages
        {
            lock (outMessages)
            {
                foreach (KeyValuePair<String, Socket> client in clientDictionary)
                {
                    outMessages.AddLast(client.Key + ":" + message);
                }
            }
            Console.WriteLine(message);
        }

        static void localChatMsg(Player currentPlayer, string message)
        // Controls local chat messages
        {
            foreach (Player player in PlayerList)
            {
                if (currentPlayer.currentRoom == player.currentRoom)
                {
                    lock (outMessages)
                    {
                        outMessages.AddLast(player.clientID + ":" + message);
                    }
                }
            }
            Console.WriteLine(message);
        }


        public static void roomUpdate(Player player, Player newPlayer, bool enteredRoom)
        // Updates rooms with whos entering and leaving
        {
            lock (outMessages)
            {
                if (enteredRoom)
                {
                    outMessages.AddLast(player.clientID + ":" +
                    "[" + newPlayer.playerName + "]" + " entered the room");
                }
                else
                {
                    outMessages.AddLast(player.clientID + ":" +
                    "[" + newPlayer.playerName + "]" + " left the room");
                }

            }
        }


        static void Main(string[] args)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            dungeon.Init();

            // Server IP 165.227.225.88
            // Local IP 127.0.0.1
            string ipAdress = "127.0.0.1"; 
            int port = 8221;

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Parse(ipAdress), port);

            serverSocket.Bind(ipLocal);
            serverSocket.Listen(4);

            Console.WriteLine("Waiting for client ..");

            var myThread = new Thread(acceptClientThread);
            myThread.Start(serverSocket);

            byte[] buffer = new byte[4096];

            while (true)
            {
                String currentMsg = "";
                lock (inMessages)
                {
                    if (inMessages.First != null)
                    {
                        currentMsg = inMessages.First.Value;

                        inMessages.RemoveFirst();

                    }
                }

                String msgToSend = "";
                lock (outMessages)
                {
                    if (outMessages.First != null)
                    {
                        msgToSend = outMessages.First.Value;

                        outMessages.RemoveFirst();

                    }
                }

                if (msgToSend != "")
                {
                    String[] substrings = msgToSend.Split(':');
                    string theClient = substrings[0];
                    string clientMsg = substrings[1];

                    byte[] sendBuffer = encoder.GetBytes(clientMsg);

                    int bytesSent = GetSocketFromName(theClient).Send(sendBuffer);

                    bytesSent = GetSocketFromName(theClient).Send(sendBuffer);
                }

                if (currentMsg != "")
                {
                    Console.WriteLine(currentMsg);
                    String[] substrings = currentMsg.Split(':');

                    int PlayerID = Int32.Parse(substrings[0]) - 1;
                    String clientMsg = substrings[1];
                    String theClient = "client" + substrings[0];
                    Player player = PlayerList[PlayerID];

                    var dungeonResult = dungeon.Process(clientMsg, player, PlayerID);

                    if (dungeonResult.Length > 7)
                    {
                        if (dungeonResult.Substring(0, 7) == "[Local]")
                        {
                            localChatMsg(player, dungeonResult);
                        }
                        else if (dungeonResult.Substring(0, 8) == "[Global]")
                        {
                            globalChatMsg(dungeonResult);
                        }
                        else
                        {
                            lock (outMessages)
                            {
                                outMessages.AddLast(theClient + ":" + dungeonResult);
                            }
                        }
                    }

                    else
                    {
                        Console.WriteLine(dungeonResult);
                        lock (outMessages)
                        {
                            outMessages.AddLast(theClient + ":" + dungeonResult);
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}