using System;

[Serializable]
public class PersistedCharacterData
{
    public string PlayerId;
    public string Name;
    public string Class;
    public long Experience;
    public byte[] ItemIds = new byte[10];
}
