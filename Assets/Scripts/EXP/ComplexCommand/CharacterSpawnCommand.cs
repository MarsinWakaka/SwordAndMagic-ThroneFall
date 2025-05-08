using System.Collections;
using System.Collections.Generic;
using GameLogic.Unit;
using GameLogic.Unit.Controller;

namespace ComplexCommand
{
    public class CharacterSpawnCommand : BaseCommand
    {
        private readonly List<BatchCharacterSpawnData> _charactersToSpawn;
        private List<CharacterUnitController> _charactersSpawned;

        public CharacterSpawnCommand(List<BatchCharacterSpawnData> charactersToSpawn)
        {
            _charactersToSpawn = charactersToSpawn;
        }

        public override IEnumerator Execute()
        {
            _charactersSpawned = new List<CharacterUnitController>();
            foreach (var record in _charactersToSpawn)
            {
                // var character = CharacterFactory.Instance.SpawnCharacter(record);
                // _charactersSpawned.Add(character);
            }
            yield return null;
        }

        protected override IEnumerator UndoSelf()
        {
            foreach (var character in _charactersSpawned)
            {
                // UnitManager.Despawn(character);
            }
            yield return null;
        }
    }
}