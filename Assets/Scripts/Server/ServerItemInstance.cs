using System;

namespace Server
{
    [Serializable]
    public class ServerItemInstance
    {
        public byte DefinitionId;
        public string CharacterName;
        public byte SlotIndex;
    }
}
