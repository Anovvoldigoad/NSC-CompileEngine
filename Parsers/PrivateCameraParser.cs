// Ported from NSC-ModManager/ViewModel/PrivateCameraViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, dialogs, RelayCommand
// StartOfFile: 0x34, struct: 0x2C per entry, semua field float kecuali CharacodeIndex (auto index)
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class PrivateCameraParser
    {
        public List<PrivateCameraModel> PrivateCameraList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => PrivateCameraList.Clear();

        public void OpenFile(string path)
        {
            Clear();
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            FilePath = path;
            FileByte = File.ReadAllBytes(path);
            int index = 128;
            string binPath = b_ReadString(FileByte, index);
            index += binPath.Length + 2;

            string binName = "";
            for (int x = 0; x < 3; x++)
            {
                string name = b_ReadString(FileByte, index);
                if (x == 0) binName = name;
                index += name.Length + 1;
            }

            if (!binName.Contains("privateCamera"))
                throw new InvalidDataException($"File is not a privateCamera xfbin. BinName={binName}");

            int startOfFile = 0x34 + b_ReadIntRev(FileByte, 16);
            int entryCount = b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 8 + (c * 0x2C);
                PrivateCameraList.Add(new PrivateCameraModel
                {
                    CharacodeIndex = c + 1,
                    CameraDistance  = b_ReadFloat(FileByte, ptr),
                    CameraSpeed     = b_ReadFloat(FileByte, ptr + 0x04),
                    CameraMovement  = b_ReadFloat(FileByte, ptr + 0x08),
                    Unk1            = b_ReadFloat(FileByte, ptr + 0x0C),
                    CameraHeight    = b_ReadFloat(FileByte, ptr + 0x10),
                    CameraAngle     = b_ReadFloat(FileByte, ptr + 0x14),
                    CameraHeight2   = b_ReadFloat(FileByte, ptr + 0x18),
                    FOV             = b_ReadFloat(FileByte, ptr + 0x1C),
                    Unk2            = b_ReadFloat(FileByte, ptr + 0x20),
                    CameraDistance2 = b_ReadFloat(FileByte, ptr + 0x24),
                    FOV2            = b_ReadFloat(FileByte, ptr + 0x28),
                });
            }
        }

        /// <summary>
        /// Merge by CharacodeIndex — overwrite kalau ada, append kalau baru.
        /// </summary>
        public void MergeWith(PrivateCameraParser modParser)
        {
            foreach (var modEntry in modParser.PrivateCameraList)
            {
                var existing = PrivateCameraList
                    .FirstOrDefault(e => e.CharacodeIndex == modEntry.CharacodeIndex);
                if (existing != null)
                {
                    existing.CameraDistance  = modEntry.CameraDistance;
                    existing.CameraSpeed     = modEntry.CameraSpeed;
                    existing.CameraMovement  = modEntry.CameraMovement;
                    existing.Unk1            = modEntry.Unk1;
                    existing.CameraHeight    = modEntry.CameraHeight;
                    existing.CameraAngle     = modEntry.CameraAngle;
                    existing.CameraHeight2   = modEntry.CameraHeight2;
                    existing.FOV             = modEntry.FOV;
                    existing.Unk2            = modEntry.Unk2;
                    existing.CameraDistance2 = modEntry.CameraDistance2;
                    existing.FOV2            = modEntry.FOV2;
                }
                else
                    PrivateCameraList.Add((PrivateCameraModel)modEntry.Clone());
            }
        }

        public void SaveFile()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                throw new InvalidOperationException("No file path set.");
            string backup = FilePath + ".backup";
            if (File.Exists(backup)) File.Delete(backup);
            File.Copy(FilePath, backup);
            File.WriteAllBytes(FilePath, ConvertToFile());
        }

        public void SaveFileTo(string outputPath)
        {
            FilePath = outputPath;
            File.WriteAllBytes(outputPath, ConvertToFile());
        }

        public byte[] ConvertToFile()
        {
            byte[] fileBytes = new byte[127]
            {
                0x4E,0x55,0x43,0x43,0x00,0x00,0x00,0x79,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x80,0xBC,0x00,0x00,0x00,0x03,0x00,0x79,0x00,0x00,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x3B,0x00,0x00,0x01,0x49,0x00,0x00,0x4C,0xE3,0x00,0x00,0x01,0x4B,
                0x00,0x00,0x0F,0x6F,0x00,0x00,0x01,0x4B,0x00,0x00,0x0F,0x84,0x00,0x00,0x05,0x20,
                0x00,0x00,0x00,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x4E,0x75,0x6C,
                0x6C,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x42,0x69,0x6E,0x61,0x72,
                0x79,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x50,0x61,0x67,0x65,0x00,
                0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x49,0x6E,0x64,0x65,0x78,0x00
            };

            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "D:/next5/char_hi/param/player/Converter/bin/privateCamera.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "privateCamera");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "Page0");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "index");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrName = fileBytes.Length;
            int addedBytes = 0;
            while (fileBytes.Length % 4 != 0) { addedBytes++; fileBytes = b_AddBytes(fileBytes, new byte[1]); }

            fileBytes = b_AddBytes(fileBytes, new byte[48]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,
                0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03
            });

            int ptrSection = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[16] { 0,0,0,0,0,0,0,1,0,0,0,2,0,0,0,3 });

            int pathLength  = ptrPath - 127;
            int nameLength  = ptrName - ptrPath;
            int section1Len = ptrSection - ptrName - addedBytes;
            int fullLength  = fileBytes.Length - 68 + 40;

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(fullLength),  16, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(2),           36, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(pathLength),  40, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),           44, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(nameLength),  48, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),           52, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(section1Len), 56, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),           60, 1);

            fileBytes = b_AddBytes(fileBytes, new byte[28]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x98,
                0x00,0x00,0x00,0x01,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x94
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x04;
            int countIndex = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[4]);
            int startPtr = fileBytes.Length;

            fileBytes = b_AddBytes(fileBytes, new byte[PrivateCameraList.Count * 0x2C]);
            for (int c = 0; c < PrivateCameraList.Count; c++)
            {
                int ptr = startPtr + (c * 0x2C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraDistance),  ptr);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraSpeed),     ptr + 0x04);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraMovement),  ptr + 0x08);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].Unk1),            ptr + 0x0C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraHeight),    ptr + 0x10);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraAngle),     ptr + 0x14);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraHeight2),   ptr + 0x18);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].FOV),             ptr + 0x1C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].Unk2),            ptr + 0x20);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].CameraDistance2), ptr + 0x24);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList[c].FOV2),            ptr + 0x28);
            }

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((PrivateCameraList.Count * 0x2C) + 8), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((PrivateCameraList.Count * 0x2C) + 4), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(PrivateCameraList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x63,0x00,0x00,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
