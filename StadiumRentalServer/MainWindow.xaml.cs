using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MongoDB.Bson;

namespace StadiumRentalServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Pokemon> dex = new List<Pokemon>();
        List<Party> Participants = new List<Party>();
        string connectionString;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

            string ConnectionFilePath = Microsoft.VisualBasic.FileSystem.CurDir() + "\\ConnectionString";
            if (File.Exists(ConnectionFilePath))
            {
                connectionString = File.ReadAllText(ConnectionFilePath);
                MongoClient dbClient;
                try
                {
                    dbClient = new MongoClient(connectionString);
                    Load_Dex();
                    Load_Parties();
                    Populate_Lists();
                }
                catch
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
        private void Load_Dex()
        {
            MongoClient dbClient = new MongoClient(connectionString);
            var db = dbClient.GetDatabase("Mons");
            var collection = db.GetCollection<BsonDocument>("Stadium2");
            var mons = collection.Find(new BsonDocument()).ToList();
            foreach (var m in mons)
            {
                string name = m.GetElement("Species").ToString().Split("=")[1];
                string gender = m.GetElement("Gender").ToString().Split("=")[1];
                string type = m.GetElement("Type").ToString().Split("=")[1];
                Dictionary<string, int> stats = new Dictionary<string, int>() {
                    { "HP", Int32.Parse(m.GetElement("HP").ToString().Split("=")[1]) },
                    { "Atk", Int32.Parse(m.GetElement("Atk").ToString().Split("=")[1]) },
                    { "Def", Int32.Parse(m.GetElement("Def").ToString().Split("=")[1]) },
                    { "Speed", Int32.Parse(m.GetElement("Speed").ToString().Split("=")[1]) },
                    { "SAtk", Int32.Parse(m.GetElement("Spec Atk").ToString().Split("=")[1]) },
                    { "SDef", Int32.Parse(m.GetElement("Spec Def").ToString().Split("=")[1]) }
                };
                string cup = m.GetElement("C-Up").ToString().Split("=")[1];
                string cdown;
                string cleft;
                string cright;
                try
                {
                    cdown = m.GetElement("C-Down").ToString().Split("=")[1];
                }
                catch
                {
                    cdown = "";
                }
                try
                {
                    cleft = m.GetElement("C-Left").ToString().Split("=")[1];
                }
                catch
                {
                    cleft = "";
                }
                try
                {
                    cright = m.GetElement("C-Right").ToString().Split("=")[1];
                }
                catch
                {
                    cright = "";
                }
                Dictionary<string, string> moves = new Dictionary<string, string>()
                {
                    {"C-Up", cup},
                    {"C-Down", cdown},
                    {"C-Left", cleft},
                    {"C-Right", cright}
                };
                Pokemon mon = new Pokemon(name, gender, type, moves, stats);
                dex.Add(mon);
            }
        }
        private void Load_Parties()
        {
            MongoClient dbClient = new MongoClient(connectionString);
            var db = dbClient.GetDatabase("Tournament");
            var collection = db.GetCollection<BsonDocument>("Parties");
            var parties = collection.Find(new BsonDocument()).ToList();
            foreach (var p in parties)
            {
                var name = p.GetValue("Party Name").ToString();
                var slot1 = dex.First(x => x.Species == p.GetValue("Slot 1").ToString());
                var slot2 = dex.First(x => x.Species == p.GetValue("Slot 2").ToString());
                var slot3 = dex.First(x => x.Species == p.GetValue("Slot 3").ToString());
                var slot4 = dex.First(x => x.Species == p.GetValue("Slot 4").ToString());
                var slot5 = dex.First(x => x.Species == p.GetValue("Slot 5").ToString());
                var slot6 = dex.First(x => x.Species == p.GetValue("Slot 6").ToString());

                try
                {
                    var battleset = p.GetValue("Battle Set") as BsonArray;
                    var first = battleset[0].ToBsonDocument().ToDictionary().Values.ToList()[0] as string;
                    var second = battleset[1].ToBsonDocument().ToDictionary().Values.ToList()[0] as string;
                    var third = battleset[1].ToBsonDocument().ToDictionary().Values.ToList()[0] as string;
                    var completeParty = new Party(name, slot1, slot2, slot3, slot4, slot5, slot6, new List<string>([first, second, third]));
                    Participants.Add(completeParty);
                }
                catch
                {
                    //this person has no battleset picked
                    var party = new Party(name, slot1, slot2, slot3, slot4, slot5, slot6);
                    Participants.Add(party);
                }
            }
        }
        private void Populate_Lists()
        {
            List_Participants_Left.Items.Clear();
            List_Participants_Right.Items.Clear();
            foreach (var participant in Participants)
            {
                List_Participants_Left.Items.Add(participant);
                List_Participants_Right.Items.Add(participant);
            }
        }

        private void List_Participants_Left_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var x = sender as ListBox;
            var selection = x.SelectedItem as Party;
            if (selection != null)
            {
                Left_Slot1_Mon.Text = selection.Slot_1.Species;
                Left_Slot2_Mon.Text = selection.Slot_2.Species;
                Left_Slot3_Mon.Text = selection.Slot_3.Species;
                Left_Slot4_Mon.Text = selection.Slot_4.Species;
                Left_Slot5_Mon.Text = selection.Slot_5.Species;
                Left_Slot6_Mon.Text = selection.Slot_6.Species;

                if (selection.Battle_Set.Count == 3)
                {
                    if (Left_Slot1_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot1_Placement.Text = "First";
                    }
                    else if (Left_Slot1_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot1_Placement.Text = "Second";
                    }
                    else if (Left_Slot1_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot1_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot1_Placement.Text = "Not Battling";
                    }

                    if (Left_Slot2_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot2_Placement.Text = "First";
                    }
                    else if (Left_Slot2_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot2_Placement.Text = "Second";
                    }
                    else if (Left_Slot2_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot2_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot2_Placement.Text = "Not Battling";
                    }

                    if (Left_Slot3_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot3_Placement.Text = "First";
                    }
                    else if (Left_Slot3_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot3_Placement.Text = "Second";
                    }
                    else if (Left_Slot3_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot3_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot3_Placement.Text = "Not Battling";
                    }

                    if (Left_Slot4_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot4_Placement.Text = "First";
                    }
                    else if (Left_Slot4_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot4_Placement.Text = "Second";
                    }
                    else if (Left_Slot4_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot4_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot4_Placement.Text = "Not Battling";
                    }

                    if (Left_Slot5_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot5_Placement.Text = "First";
                    }
                    else if (Left_Slot5_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot5_Placement.Text = "Second";
                    }
                    else if (Left_Slot5_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot5_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot5_Placement.Text = "Not Battling";
                    }

                    if (Left_Slot6_Mon.Text == selection.Battle_Set[0])
                    {
                        Left_Slot6_Placement.Text = "First";
                    }
                    else if (Left_Slot6_Mon.Text == selection.Battle_Set[1])
                    {
                        Left_Slot6_Placement.Text = "Second";
                    }
                    else if (Left_Slot6_Mon.Text == selection.Battle_Set[2])
                    {
                        Left_Slot6_Placement.Text = "Third";
                    }
                    else
                    {
                        Left_Slot6_Placement.Text = "Not Battling";
                    }
                }
            }
        }

        private void List_Participants_Right_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var x = sender as ListBox;
            var selection = x.SelectedItem as Party;
            if (selection != null)
            {
                Right_Slot1_Mon.Text = selection.Slot_1.Species;
                Right_Slot2_Mon.Text = selection.Slot_2.Species;
                Right_Slot3_Mon.Text = selection.Slot_3.Species;
                Right_Slot4_Mon.Text = selection.Slot_4.Species;
                Right_Slot5_Mon.Text = selection.Slot_5.Species;
                Right_Slot6_Mon.Text = selection.Slot_6.Species;

                if (selection.Battle_Set.Count == 3)
                {
                    if (Right_Slot1_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot1_Placement.Text = "First";
                    }
                    else if (Right_Slot1_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot1_Placement.Text = "Second";
                    }
                    else if (Right_Slot1_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot1_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot1_Placement.Text = "Not Battling";
                    }

                    if (Right_Slot2_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot2_Placement.Text = "First";
                    }
                    else if (Right_Slot2_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot2_Placement.Text = "Second";
                    }
                    else if (Right_Slot2_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot2_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot2_Placement.Text = "Not Battling";
                    }

                    if (Right_Slot3_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot3_Placement.Text = "First";
                    }
                    else if (Right_Slot3_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot3_Placement.Text = "Second";
                    }
                    else if (Right_Slot3_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot3_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot3_Placement.Text = "Not Battling";
                    }

                    if (Right_Slot4_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot4_Placement.Text = "First";
                    }
                    else if (Right_Slot4_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot4_Placement.Text = "Second";
                    }
                    else if (Right_Slot4_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot4_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot4_Placement.Text = "Not Battling";
                    }

                    if (Right_Slot5_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot5_Placement.Text = "First";
                    }
                    else if (Right_Slot5_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot5_Placement.Text = "Second";
                    }
                    else if (Right_Slot5_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot5_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot5_Placement.Text = "Not Battling";
                    }

                    if (Right_Slot6_Mon.Text == selection.Battle_Set[0])
                    {
                        Right_Slot6_Placement.Text = "First";
                    }
                    else if (Right_Slot6_Mon.Text == selection.Battle_Set[1])
                    {
                        Right_Slot6_Placement.Text = "Second";
                    }
                    else if (Right_Slot6_Mon.Text == selection.Battle_Set[2])
                    {
                        Right_Slot6_Placement.Text = "Third";
                    }
                    else
                    {
                        Right_Slot6_Placement.Text = "Not Battling";
                    }
                }
            }
        }

        private void Pull_Input_Click(object sender, RoutedEventArgs e)
        {
            MongoClient dbClient = new MongoClient(connectionString);
            var db = dbClient.GetDatabase("Tournament");
            var collection = db.GetCollection<BsonDocument>("Inputs");
            var left_input = collection.Find(new BsonDocument("Party Name", List_Participants_Left.SelectedItem.ToString())).ToList();
            var right_input = collection.Find(new BsonDocument("Party Name", List_Participants_Right.SelectedItem.ToString())).ToList();

            if (left_input.Count != 0) 
            {
                Left_Input.Text = List_Participants_Left.SelectedItem.ToString() + "\nhas selected\n" + left_input[0].GetValue("Input").ToString();
            }
            else
            {
                Left_Input.Text = List_Participants_Left.SelectedItem.ToString() + "\nhas not chosen an input";
            }

            if (right_input.Count != 0)
            {
                Right_Input.Text = List_Participants_Right.SelectedItem.ToString() + "\nhas selected\n" + right_input[0].GetValue("Input").ToString();
            }
            else
            {
                Right_Input.Text = List_Participants_Right.SelectedItem.ToString() + "\nhas not chosen an input";
            }
        }

        private void Consume_Input_Click(object sender, RoutedEventArgs e)
        {
            MongoClient dbClient = new MongoClient(connectionString);
            var db = dbClient.GetDatabase("Tournament");
            var collection = db.GetCollection<BsonDocument>("Inputs");
            collection.DeleteOne(new BsonDocument("Party Name", List_Participants_Left.SelectedItem.ToString()));
            collection.DeleteOne(new BsonDocument("Party Name", List_Participants_Right.SelectedItem.ToString()));
        }

        private void Reload_Parties_Click(object sender, RoutedEventArgs e)
        {
            Participants.Clear();
            Load_Parties();
            Populate_Lists();
        }
    }
    public class Pokemon
    {
        public string Species { get; set; }
        public string Gender { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Moves { get; set; }

        public Dictionary<string, int>? Stats { get; set; }
        private static List<string> Stat_Names = new List<string> { "hp", "atk", "spec", "def", "speed" };
        public Pokemon()
        {
            Species = "";
            Gender = "NB";
            Type = "";
            Moves = new Dictionary<string, string>();
            Stats = Stat_Names.ToDictionary(k => k, k => 0);
        }
        public Pokemon(string species, string gender, Dictionary<string, string> moves)
        {
            Species = species;
            Gender = gender;
            Type = "";
            Moves = moves;

        }
        public Pokemon(string species, string gender, string type, Dictionary<string, string> moves)
        {
            Species = species;
            Gender = gender;
            Type = type;
            Moves = moves;
        }
        public Pokemon(string species, string gender, string type, Dictionary<string, string> moves, Dictionary<string, int>? stats) : this(species, gender, type, moves)
        {
            Stats = stats;
        }
        public override string ToString()
        {
            return Species;
        }
    }

    public class Party
    {
        public string Name { get; set; }
        public Pokemon Slot_1 { get; set; }
        public Pokemon Slot_2 { get; set; }
        public Pokemon Slot_3 { get; set; }
        public Pokemon Slot_4 { get; set; }
        public Pokemon Slot_5 { get; set; }
        public Pokemon Slot_6 { get; set; }
        public List<string> Battle_Set { get; set; }

        public Party()
        {
            Name = string.Empty;
            Slot_1 = new Pokemon();
            Slot_2 = new Pokemon();
            Slot_3 = new Pokemon();
            Slot_4 = new Pokemon();
            Slot_5 = new Pokemon();
            Slot_6 = new Pokemon();
            Battle_Set = new List<string>();
        }
        public Party(string name, Pokemon slot_1, Pokemon slot_2, Pokemon slot_3, Pokemon slot_4, Pokemon slot_5, Pokemon slot_6)
        {
            Name = name;
            Slot_1 = slot_1;
            Slot_2 = slot_2;
            Slot_3 = slot_3;
            Slot_4 = slot_4;
            Slot_5 = slot_5;
            Slot_6 = slot_6;
            Battle_Set = new List<string>();
        }
        public Party(string name, Pokemon slot_1, Pokemon slot_2, Pokemon slot_3, Pokemon slot_4, Pokemon slot_5, Pokemon slot_6, List<string> battle_Set) : this(name, slot_1, slot_2, slot_3, slot_4, slot_5, slot_6)
        {
            Battle_Set = battle_Set;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}