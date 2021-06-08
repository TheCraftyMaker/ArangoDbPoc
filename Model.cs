using System.Collections.Generic;

namespace ArangoDbPoc
{
    record Character(string Name, string Surname, bool Alive, int Age, List<string> Traits);
}
