using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using Hooks;
namespace TShockBroadcast
{
    [APIVersion(1, 12)]
    public class TShockBroadcast : TerrariaPlugin
    {
        public static int interval = 15;
        //public static tbclist messages;
        public static int mode = 0; // 0 -seq, 1 - rand

        public static DateTime timer = DateTime.UtcNow;
        public static int counter = 0;
        public static int current = 0;

        public static tbclist messages = new tbclist();

        public override string Name
        {
            get { return "TBroadcast"; }
        }

        public override Version Version
        {
            get { return new Version("1.0.0.0"); }
        }

        public override string Author
        {
            get { return "by PureMango"; }
        }

        public override string Description
        {
            get { return "Autobroadcasts your messages consequently or randomly, using 1 timer"; }
        }

        public override void Initialize()
        {
            GameHooks.Initialize += OnInitialize;
            GameHooks.Update += OnUpdate;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Initialize -= OnInitialize;
                GameHooks.Update -= OnUpdate;
            }
            base.Dispose(disposing);
        }

        public static void tbcommand(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Usage: /tbc toggle - To switch mode seq/random", Color.Red);
                return;
            }

            string subcmd = args.Parameters[0].ToLower();

            if (subcmd == "toggle")
            {
                try
                {
                    if (mode == 0)
                    {
                        mode = 1;
                        args.Player.SendMessage("TBroadcast mode set to random", Color.MediumSeaGreen);
                    }
                    else
                    {
                        mode = 0;
                        args.Player.SendMessage("TBroadcast mode set to sequence", Color.MediumSeaGreen);
                    }
                }
                catch (Exception ex)
                {
                    args.Player.SendMessage("Error, Check Logs!", Color.Red);
                    Log.Error("TBroadcast Error");
                    Log.Error(ex.ToString());
                }
            }
            else
            {
                args.Player.SendMessage("Usage: /tbc toggle - To switch mode seq/random", Color.Red);
            }
        }

        public TShockBroadcast(Main game) : base(game)
        {
            Order = 4;
        }

        public void OnInitialize()
        {
            timer = DateTime.UtcNow;

            List<string> msg1 = new List<string>();
            msg1.Add("Cool Party");
            msg1.Add("Super Party");
            msg1.Add("Duper Party");

            List<string> msg2 = new List<string>();
            msg2.Add("Cool Pool");
            msg2.Add("Super Pool");
            msg2.Add("Duper Pool");

            List<string> msg3 = new List<string>();
            msg3.Add("Pretty test");
            msg3.Add("Nice test");
            msg3.Add("Pinky ponky");

            messages.TShockBroadcast.Add(new tbc("Test1", msg1, 50, 50, 50));
            messages.TShockBroadcast.Add(new tbc("Test2", msg2, 0, 255, 255));
            messages.TShockBroadcast.Add(new tbc("Test3", msg3, 255, 255, 0));

            Commands.ChatCommands.Add(new Command("tbroadcast", tbcommand, "tbc"));
        }

        public void OnUpdate()
        {
            if((DateTime.UtcNow - timer).TotalMilliseconds >= 1000)
            {
                timer = DateTime.UtcNow;
                if(counter < interval)
                {
                    counter++;
                }
                else if(counter == interval)
                {
                    BroadcastMessage(mode);
                    counter = 0;
                }
            }
        }

        public void BroadcastMessage(int mode)
        {
            Random rand = new Random();
            int num = 0;

            try
            {
                if (messages == null) return;

                if (mode == 0) //sequence
                {
                    EchoMessage(current);

                    if (current == (messages.TShockBroadcast.Count() - 1))
                    {
                        current = 0;
                    }
                    else
                    {
                        current++;
                    }
                }
                else if (mode == 1) //random
                {
                    num = rand.Next(0, messages.TShockBroadcast.Count());
                    EchoMessage(num);
                }
            }
            catch { }
        }

        public void EchoMessage(int msgId)
        {
            byte r = messages.TShockBroadcast[msgId].ColorR;
            byte g = messages.TShockBroadcast[msgId].ColorG;
            byte b = messages.TShockBroadcast[msgId].ColorB;

            foreach (string msg in messages.TShockBroadcast[msgId].Messages)
            {
                if(msg == null) continue;
                TSPlayer.All.SendMessage(msg, r, g, b);
            }
        }

        public class tbclist
        {
            public List<tbc> TShockBroadcast;

            public tbclist()
            {
                TShockBroadcast = new List<tbc>();
            }
        }

        public class tbc
        {
            public string Name;
            //public bool Enabled;
            public List<string> Messages;
            public byte ColorR;
            public byte ColorG;
            public byte ColorB;
            //public List<string> Groups;

            public tbc(string Name, List<string> Messages, byte ColorR, byte ColorG, byte ColorB)
            {
                this.Name = Name;
                //this.Enabled = Enabled;
                this.Messages = Messages;
                this.ColorR = ColorR;
                this.ColorG = ColorG;
                this.ColorB = ColorB;
                //this.Groups = Groups;
            }
        }
    }
}
