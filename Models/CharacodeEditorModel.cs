// Ported from NSC-ModManager/Model/CharacodeEditorModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class CharacodeEditorModel : ICloneable
    {
        public string CharacodeName { get; set; } = "";
        public int CharacodeIndex { get; set; }

        public object Clone() => new CharacodeEditorModel
        {
            CharacodeName = CharacodeName,
            CharacodeIndex = CharacodeIndex,
        };
    }
}
