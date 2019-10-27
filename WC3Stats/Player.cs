using System.Threading.Tasks;

namespace WC3Stats
{
    public enum Team
    {
        TeamOne,
        TeamTwo
    }
    public class Player
    {
        public Player(string name, Races race, int id, bool isMe, byte[] bytes)
        {
            Name = name;
            Race = race;
            Id = id;
            IsMe = isMe;
            Bytes = bytes;
        }

        public Team Team => Id % 2 == 0 ? Team.TeamOne : Team.TeamTwo;

        public string Name { get; }
        public Races Race { get; }

        public int Id { get; set; }

        public PlayerStats PlayerStats { get; set; }
        public bool IsMe { get; }
        public byte[] Bytes { get; }

        public static Player ParsePlayer(byte[] bytes, int index, bool isMe)
        {
            var nameBytes = bytes.TakeFromIndexWhileNotZeroByte(index);
            var name = nameBytes.AsString();

            var raceOffset = index + name.Length + 6;
            var race = (Races)bytes[raceOffset];
            int id = bytes[index - 1];

            return new Player(name, race, id, isMe, bytes);
        }
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Race)}: {Race}, {nameof(Id)}: {Id}";
        }
    }
}