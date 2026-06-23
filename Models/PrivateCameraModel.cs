// Ported from NSC-ModManager/Model/PrivateCameraModel.cs
// Stripped: INotifyPropertyChanged → pure POCO
namespace NSC_CompileEngine.Models
{
    public class PrivateCameraModel : ICloneable
    {
        public int CharacodeIndex { get; set; }
        public float CameraDistance { get; set; }
        public float CameraSpeed { get; set; }
        public float CameraMovement { get; set; }
        public float Unk1 { get; set; }
        public float CameraHeight { get; set; }
        public float CameraAngle { get; set; }
        public float CameraHeight2 { get; set; }
        public float FOV { get; set; }
        public float Unk2 { get; set; }
        public float CameraDistance2 { get; set; }
        public float FOV2 { get; set; }

        public object Clone() => new PrivateCameraModel
        {
            CharacodeIndex = CharacodeIndex,
            CameraDistance = CameraDistance, CameraSpeed = CameraSpeed,
            CameraMovement = CameraMovement, Unk1 = Unk1,
            CameraHeight = CameraHeight, CameraAngle = CameraAngle,
            CameraHeight2 = CameraHeight2, FOV = FOV,
            Unk2 = Unk2, CameraDistance2 = CameraDistance2, FOV2 = FOV2,
        };
    }
}
