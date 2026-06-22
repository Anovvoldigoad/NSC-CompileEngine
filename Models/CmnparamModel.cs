// Ported from NSC-ModManager/Model/cmnparamModel.cs
// Stripped: INotifyPropertyChanged → plain POCO, no WPF dependency
namespace NSC_CompileEngine.Models
{
    public class PairSplSndModel : ICloneable
    {
        public int PairSplID { get; set; }
        public string PairSoundEvFileName { get; set; } = "";
        public string PairCutInChunkName { get; set; } = "";
        public string PairAtkChunkName { get; set; } = "";
        public string PairSplName1 { get; set; } = "";
        public string PairSplName2 { get; set; } = "";
        public object Clone() => new PairSplSndModel {
            PairSplID = PairSplID, PairSoundEvFileName = PairSoundEvFileName,
            PairCutInChunkName = PairCutInChunkName, PairAtkChunkName = PairAtkChunkName,
            PairSplName1 = PairSplName1, PairSplName2 = PairSplName2,
        };
    }

    public class PlayerSndModel : ICloneable
    {
        public string PlayerCharacode { get; set; } = "";
        public string PlayerSndBaseFileName { get; set; } = "";
        public string PlayerSndAwa1FileName { get; set; } = "";
        public string PlayerSndAwa2FileName { get; set; } = "";
        public string PlayerSndEventFileName { get; set; } = "";
        public string PlayerSndUJEventFileName { get; set; } = "";
        public string PlayerSndUJ_1_CutIn_ChunkName { get; set; } = "";
        public string PlayerSndUJ_1_Atk_ChunkName { get; set; } = "";
        public string PlayerSndUJ_2_CutIn_ChunkName { get; set; } = "";
        public string PlayerSndUJ_2_Atk_ChunkName { get; set; } = "";
        public string PlayerSndUJ_3_CutIn_ChunkName { get; set; } = "";
        public string PlayerSndUJ_3_Atk_ChunkName { get; set; } = "";
        public string PlayerSndUJ_alt_CutIn_ChunkName { get; set; } = "";
        public string PlayerSndUJ_alt_Atk_ChunkName { get; set; } = "";
        public string PlayerPartnerCharacodeBase { get; set; } = "";
        public string PlayerPartnerCharacodeAwake { get; set; } = "";
        public object Clone() => new PlayerSndModel {
            PlayerCharacode = PlayerCharacode, PlayerSndBaseFileName = PlayerSndBaseFileName,
            PlayerSndAwa1FileName = PlayerSndAwa1FileName, PlayerSndAwa2FileName = PlayerSndAwa2FileName,
            PlayerSndEventFileName = PlayerSndEventFileName, PlayerSndUJEventFileName = PlayerSndUJEventFileName,
            PlayerSndUJ_1_CutIn_ChunkName = PlayerSndUJ_1_CutIn_ChunkName, PlayerSndUJ_1_Atk_ChunkName = PlayerSndUJ_1_Atk_ChunkName,
            PlayerSndUJ_2_CutIn_ChunkName = PlayerSndUJ_2_CutIn_ChunkName, PlayerSndUJ_2_Atk_ChunkName = PlayerSndUJ_2_Atk_ChunkName,
            PlayerSndUJ_3_CutIn_ChunkName = PlayerSndUJ_3_CutIn_ChunkName, PlayerSndUJ_3_Atk_ChunkName = PlayerSndUJ_3_Atk_ChunkName,
            PlayerSndUJ_alt_CutIn_ChunkName = PlayerSndUJ_alt_CutIn_ChunkName, PlayerSndUJ_alt_Atk_ChunkName = PlayerSndUJ_alt_Atk_ChunkName,
            PlayerPartnerCharacodeBase = PlayerPartnerCharacodeBase, PlayerPartnerCharacodeAwake = PlayerPartnerCharacodeAwake,
        };
    }
}
