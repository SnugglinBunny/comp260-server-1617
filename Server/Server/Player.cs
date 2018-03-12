using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Player
    {
        public Dungeon dungeonRef;
        public Room currentRoom;
        public String playerName;
        public Int32 Health = 100;
        public String[] inventory = new String[5];

        public void Init()
        {
            currentRoom = dungeonRef.roomMap["Room 1"]; ;
        }
    }
}