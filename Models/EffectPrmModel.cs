// Ported from NSC-ModManager/Model/EffectPrmModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class EffectPrmModel : ICloneable
    {
        public int EffectPrmID { get; set; }
        public int Type { get; set; }
        public string FilePath { get; set; } = "";
        public string ChunkName { get; set; } = "";

        public object Clone() => new EffectPrmModel
        {
            EffectPrmID = EffectPrmID,
            Type = Type,
            FilePath = FilePath,
            ChunkName = ChunkName,
        };
    }
}
