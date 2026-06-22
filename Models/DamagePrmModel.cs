// Ported from NSC-ModManager/Model/DamagePrmModel.cs
// Stripped: INotifyPropertyChanged, OnPropertyChanged — pure data holder
namespace NSC_CompileEngine.Models
{
    public class DamagePrmModel : ICloneable
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public object Clone() => new DamagePrmModel { Data = (byte[])Data.Clone() };
    }
}
