// Ported from NSC-ModManager/Model/SupportSkillRecoverySpeedParamModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class SupportSkillRecoverySpeedParamModel : ICloneable
    {
        public int CharacodeID { get; set; }
        public float Jutsu1 { get; set; }
        public float Jutsu2 { get; set; }
        public float Jutsu3 { get; set; }
        public float Jutsu4 { get; set; }
        public float Jutsu5 { get; set; }
        public float Jutsu6 { get; set; }
        public float Jutsu1_awa { get; set; }
        public float Jutsu2_awa { get; set; }

        public object Clone() => new SupportSkillRecoverySpeedParamModel
        {
            CharacodeID = CharacodeID,
            Jutsu1 = Jutsu1, Jutsu2 = Jutsu2, Jutsu3 = Jutsu3,
            Jutsu4 = Jutsu4, Jutsu5 = Jutsu5, Jutsu6 = Jutsu6,
            Jutsu1_awa = Jutsu1_awa, Jutsu2_awa = Jutsu2_awa,
        };
    }
}
