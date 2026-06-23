// Ported from NSC-ModManager/Model/SkillIndexSettingParamModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class SkillIndexSettingParamModel : ICloneable
    {
        public int CharacodeID { get; set; }
        public int JutsuIndex1 { get; set; }
        public int JutsuIndex2 { get; set; }

        public object Clone() => new SkillIndexSettingParamModel
        {
            CharacodeID = CharacodeID,
            JutsuIndex1 = JutsuIndex1,
            JutsuIndex2 = JutsuIndex2,
        };
    }
}
