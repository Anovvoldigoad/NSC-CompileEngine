// Ported from NSC-ModManager/Model/CostumeParamModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class CostumeParamModel : ICloneable
    {
        public string EntryString { get; set; } = "";
        public int EntryIndex { get; set; }
        public int PlayerSettingParamID { get; set; }
        public string CharacterName { get; set; } = "";
        public int EntryType { get; set; }
        public int UnlockCost { get; set; }
        public int UnlockCondition { get; set; }

        public object Clone() => new CostumeParamModel
        {
            EntryString          = EntryString,
            EntryIndex           = EntryIndex,
            PlayerSettingParamID = PlayerSettingParamID,
            CharacterName        = CharacterName,
            EntryType            = EntryType,
            UnlockCost           = UnlockCost,
            UnlockCondition      = UnlockCondition,
        };
    }
}
