using SampleApp.GuardiansOfTheGalaxy.Models;

namespace SampleApp.GuardiansOfTheGalaxy.Services
{
    internal class GuardianCreator : IGuardianCreator
    {
        private readonly string[] _guardians = new[]
        {
            "Star-Lord",
            "Gemora",
            "Drax the Destroyer",
            "Groot",
            "Rocket",
            "Ronan the Accuser",
            "Yondu Udonta",
            "Nebula"
        };

        private readonly string[] _weapons = new[]
        {
            "Mantis' Twin Blades",
            "Groot's Roots",
            "Rocket's Cluster Grenade",
            "Rocket's Heavy Blaster",
            "Rocket's Ultimate Blaster",
            "Drax's Katathian Daggers",
            "Gamora's Assassin Sword",
            "Spartoi Element Guns"
        };

        public Task<Guardian> Create(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var random = new Random();
                var name = _guardians.ElementAt(random.Next(0, _guardians.Length - 1));
                var weapon = RetrieveWeapon();

                return new Guardian(name, weapon);

            }, cancellationToken);
        }


        private Weapon RetrieveWeapon()
        {
            var random = new Random();
            var weapon = _weapons.ElementAt(random.Next(0, _guardians.Length - 1));
            var damage = (random.NextDouble() + 1) * 100;

            return new Weapon(weapon, damage);
        }
    }
}
