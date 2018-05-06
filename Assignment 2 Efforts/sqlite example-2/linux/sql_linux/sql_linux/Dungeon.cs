using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
    public class Dungeon
    {
        sqliteConnection conn = null;
        string databaseName = "./JR191353/MUDData.database"; // Creates a string with the database name

        string currentRoom;

        public void Init()
        {
            var roomMap = new Dictionary<string, Room>(); // Creates dictionary of rooms
            {
                var room = new Room("Room 1", "You are standing in the entrance hall\nAll adventures start here");
                room.north = "Room 2";
                room.south = "Room 11";
                room.west = "Room 5";
                room.east = "Room 6";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 2", "You are in room 2");
                room.south = "Room 1";
                room.west = "Room 3";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 3", "You are in room 3");
                room.south = "Room 5";
                room.east = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 4", "As you enter the room you slip and hit your head. Game Over.");
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 5", "You are in room 5");
                room.south = "Room 12";
                room.east = "Room 1";
                room.north = "Room 3";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 6", "You are in room 6");
                room.north = "Room 7";
                room.east = "Room 8";
                room.west = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 7", "You are in room 7");
                room.south = "Room 6";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 8", "You are in room 8");
                room.south = "Room 9";
                room.west = "Room 6";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 9", "You are in room 9");
                room.north = "Room 8";
                room.west = "Room 10";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 10", "You are in room 10");
                room.east = "Room 9";
                room.west = "Room 11";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 11", "You are in room 11");
                room.north = "Room 1";
                room.east = "Room 10";
                room.west = "Room 12";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 12", "You are in room 12");
                room.north = "Room 5";
                room.east = "Room 11";
                room.west = "Room 13";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 13", "You are in room 13");
                room.south = "Room 14";
                room.east = "Room 12";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 14", "You are in room 14");
                room.north = "Room 13";
                room.south = "Room 15";
                room.east = "Room 16";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 15", "You are in room 15");
                room.north = "Room 14";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 16", "You are in room 16");
                room.east = "Room 17";
                room.west = "Room 14";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 17", "You are in room 17");
                room.south = "Room 18";
                room.west = "Room 16";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 18", "You are in room 18");
                room.north = "Room 17";
                room.south = "Room 19";
                room.east = "Room 21";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 19", "You are in room 19");
                room.north = "Room 18";
                room.east = "Room 21";
                room.west = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 21", "You are in room 20");
                room.north = "Room 10";
                room.south = "Room 19";
                room.west = "Room 18";
                roomMap.Add(room.name, room);
            }
            try
            {
                sqliteConnection.CreateFile(databaseName); // Creates database file using the name string

                conn = new sqliteConnection("Data Source=" + databaseName + ";Version=3;FailIfMissing=True"); // Inits connection to the database

                sqliteCommand command; // Inits a new sql command

                conn.Open(); // Opens new connection

                command = new sqliteCommand("create table table_rooms (name varchar(20), desc varchar(20), north varchar(20), south varchar(20), west varchar(20), east varchar(20))", conn);
                command.ExecuteNonQuery(); // cCeates and executes sql command which creates the new rooms table

                foreach (var kvp in roomMap) // Here we go through each room in the dictionary and add it to the rooms table
                {
                    try
                    {
                        var sql = "insert into " + "table_rooms" + " (name, desc, north, south, west, east) values "; // Parsing the data into something sql can understand
                        sql += "('" + kvp.Key + "'";
                        sql += ",";
                        sql += "'" + kvp.Value.desc + "'";
                        sql += ",";
                        sql += "'" + kvp.Value.north + "'";
                        sql += ",";
                        sql += "'" + kvp.Value.south + "'";
                        sql += ",";
                        sql += "'" + kvp.Value.west + "'";
                        sql += ",";
                        sql += "'" + kvp.Value.east + "'";
                        sql += ")";

                        command = new sqliteCommand(sql, conn);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to add room\n" + ex); // Prints if there is an error
                    }
                }
                try
                {
                    Console.WriteLine("");
                    command = new sqliteCommand("select * from " + "table_rooms", conn); // Cannot use " order by name asc" as it doesn't order numbers correctly
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine("Name: " + reader["name"] + " " + "Exits: " + reader["north"] + " " +
                            reader["south"] + " " + reader["west"] + " " + reader["east"]); // Prints each room added to table
                    }

                    reader.Close();
                    Console.WriteLine("");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to display DB " + ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create DB failed: " + ex);
            }

            currentRoom = "Room 1"; // Sets the current room for each new player to room 1

        }

        public void roomUpdate(Player currentPlayer, string previousRoom)
        {
            foreach (Player player in server.PlayerList)
            {
                if (player.currentRoom == previousRoom)
                {
                    server.roomUpdate(player, currentPlayer, false);
                }
            }
        }

        public String DungeonInfo(Player currentPlayer, bool enteredNewRoom)
        {
            String info = "";
            currentRoom = currentPlayer.currentRoom;

            sqliteCommand command;

            command = new sqliteCommand("select * from  table_rooms where name == '" + currentRoom + "'", conn);
            var reader = command.ExecuteReader();
            bool newPlayer = false;

            while (reader.Read())
            {
                String[] temp = { "north", "south", "east", "west" };

                if (enteredNewRoom)
                {
                    newPlayer = true;
                    info += reader["desc"];
                    info += "\nExits\n";

                    for (var i = 0; i < temp.Length; i++)
                    {
                        string result = reader[temp[i]] as String;
                        if (result != "")
                        {
                            info += (reader[temp[i]] + " " + temp[i] + "\n");
                        }
                    }
                }
            }

            info += ("\n> ");

            int Players = 1;

            foreach (Player player in server.PlayerList)
            {

                if (currentPlayer.currentRoom == player.currentRoom)
                {
                    if (player != currentPlayer)
                    {

                        Players++;
                        if (Players == 2)
                        {
                            info += "\nOther players here ->";
                            info += " [" + player.playerName + "]";
                            if (newPlayer)
                            {
                                server.roomUpdate(player, currentPlayer, true);

                            }
                        }
                        else
                        {
                            info += " [" + player.playerName + "]";
                            if (newPlayer)
                            {
                                server.roomUpdate(player, currentPlayer, true);
                            }
                        }
                    }
                }
            }

            if (Players == 1) { info += "\nYou're alone!\n"; }
            else { info += "\n"; }

            return info;

        }

        public string Process(string Key, Player player, int PlayerID)
        {
            currentRoom = player.currentRoom; // Sets currentRoom for each player
            var command = new sqliteCommand("select * from  table_rooms where name == '" + currentRoom + "'", conn);
            var reader = command.ExecuteReader();
            DungeonInfo(player, false); // Displays the dungeon info to player
            String returnString = "";
            var input = Key.Split(' ');

            switch (input[0].ToLower())
            {
                case "help":
                    returnString += ("\nCommands are ....");
                    returnString += ("\nhelp - for this screen");
                    returnString += ("\nlook - to look around");
                    returnString += ("\ngo [north | south | east | west]  - to travel between locations");
                    returnString += ("\nPress any key to continue");
                    returnString += ("\nname - to set name your name");
                    returnString += ("\nsay - global chat");
                    returnString += ("\nlocal - local chat\n\n" + DungeonInfo(player, true));
                    return returnString;

                case "name":
                    String newName = "";
                    for (var i = 1; i < input.Length; i++)
                    {
                        newName += (input[i]);
                    }
                    player.playerName = (newName);
                    returnString += ("\nNice to meet you " + newName + "\n\n" + DungeonInfo(player, true));
                    return returnString;

                case "look":
                    Thread.Sleep(500);
                    returnString = DungeonInfo(player, false);
                    return returnString;

                case "local":
                    returnString += ("[local][" + player.playerName + "]");
                    for (var i = 1; i < input.Length; i++)
                    {
                        returnString += (input[i] + " ");
                    }

                    Thread.Sleep(1000);
                    return returnString;

                case "say":
                    returnString += ("[global][" + player.playerName + "]");
                    for (var i = 1; i < input.Length; i++)
                    {
                        returnString += (input[i] + " ");
                    }

                    Thread.Sleep(1000);
                    return returnString;

                case "go":
                    while (reader.Read())
                    {
                        if ((input[1].ToLower() == "north") && (reader["north"] != null)) // If user says go north
                        {
                            currentRoom = reader["north"].ToString(); // Attempts to go north
                        }
                        else
                        {
                            if ((input[1].ToLower() == "south") && (reader["south"] != null))
                            {
                                currentRoom = reader["south"].ToString();
                            }
                            else
                            {
                                if ((input[1].ToLower() == "east") && (reader["east"] != null))
                                {
                                    currentRoom = reader["east"].ToString();
                                }
                                else
                                {
                                    if ((input[1].ToLower() == "west") && (reader["west"] != null))
                                    {
                                        currentRoom = reader["west"].ToString();
                                    }
                                    else
                                    {
                                        // Handles Errors
                                        Console.WriteLine("\nERROR");
                                        Console.WriteLine("\nCan not go " + input[1] + " from here");
                                        Console.WriteLine("\nPress any key to continue");
                                        Console.ReadKey(true);
                                    }
                                }
                            }
                        }
                        player.currentRoom = currentRoom;
                        bool error = false;
                        if (!error)
                        {
                            roomUpdate(player, currentRoom);
                        }

                    }
                    returnString += DungeonInfo(player, true);
                    return returnString;

                default: // This is the default message if something is typed that doesn't match a command like above
                    returnString += ("\nERROR");
                    returnString += ("\nCan not " + Key);
                    returnString += ("\nPress any key to continue");

                    return returnString;
            }

            return returnString;

        }
    }
}