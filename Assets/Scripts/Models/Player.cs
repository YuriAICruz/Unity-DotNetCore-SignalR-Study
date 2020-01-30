namespace DefaultNamespace.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Player()
        {
        }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"id: {Id} name: {Name}";
        }
    }
}