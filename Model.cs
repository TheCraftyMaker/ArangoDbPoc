using System.Collections.Generic;

namespace ArangoDbPoc
{
    internal record Character(string Name, string Surname, bool Alive, int Age, List<string> Traits);
    internal record Weapon(string Type, string Name);
    internal record CharacterRelationship(string ParentName, string ParentSurname, string ChildName, string ChildSurname);
    internal record WeaponRelationship(string CharacterName, string CharacterSurname, string WeaponName);
}
