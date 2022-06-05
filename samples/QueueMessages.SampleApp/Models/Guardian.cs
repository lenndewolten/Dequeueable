namespace SampleApp.GuardiansOfTheGalaxy.Models
{
    internal record Guardian
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }

        public Weapon Weapon { get; }

        public Guardian(Guid id, string name, Weapon weapon)
            : this(name, weapon)
        {
            Id = id;
        }

        public Guardian(string name, Weapon weapon)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        }
    }
}
