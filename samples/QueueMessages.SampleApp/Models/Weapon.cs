namespace SampleApp.GuardiansOfTheGalaxy.Models
{
    internal record Weapon
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public double Damage { get; }

        public Weapon(Guid id, string name, double damage)
            : this(name, damage)
        {
            Id = id;
        }

        public Weapon(string name, double damage)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Damage = damage;
        }
    }
}
