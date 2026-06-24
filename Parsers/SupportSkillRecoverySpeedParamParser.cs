// Ported from NSC-ModManager/ViewModel/SupportSkillRecoverySpeedParamViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, dialogs, RelayCommand
// StartOfFile: 0x44, struct: 0x24 per entry, header 40+16 bytes
// BUG ASLI DIPERTAHANKAN: offset 0x20 tulis Jutsu1_awa bukan Jutsu2_awa
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class SupportSkillRecoverySpeedParamParser
    {
        public List<SupportSkillRecoverySpeedParamModel> SupportSkillRecoverySpeedParamList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => SupportSkillRecoverySpeedParamList.Clear();

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

            if (!binName.Contains("supportSkillRecoverySpeedParam"))
                throw new InvalidDataException($"Not a supportSkillRecoverySpeedParam xfbin. BinName={binName}");

            int startOfFile = 0x44 + b_ReadIntRev(FileByte, 16);
            int entryCount = b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 0x10 + (c * 0x24);
                SupportSkillRecoverySpeedParamList.Add(new SupportSkillRecoverySpeedParamModel
                {
                    CharacodeID = b_ReadInt(FileByte, ptr),
                    Jutsu1      = b_ReadFloat(FileByte, ptr + 0x04),
                    Jutsu2      = b_ReadFloat(FileByte, ptr + 0x08),
                    Jutsu3      = b_ReadFloat(FileByte, ptr + 0x0C),
                    Jutsu4      = b_ReadFloat(FileByte, ptr + 0x10),
                    Jutsu5      = b_ReadFloat(FileByte, ptr + 0x14),
                    Jutsu6      = b_ReadFloat(FileByte, ptr + 0x18),
                    Jutsu1_awa  = b_ReadFloat(FileByte, ptr + 0x1C),
                    Jutsu2_awa  = b_ReadFloat(FileByte, ptr + 0x20),
                });
            }
        }

        public void MergeWith(SupportSkillRecoverySpeedParamParser modParser)
        {
            foreach (var modEntry in modParser.SupportSkillRecoverySpeedParamList)
            {
                var existing = SupportSkillRecoverySpeedParamList
                    .FirstOrDefault(e => e.CharacodeID == modEntry.CharacodeID);
                if (existing != null)
                {
                    existing.Jutsu1     = modEntry.Jutsu1;
                    existing.Jutsu2     = modEntry.Jutsu2;
                    existing.Jutsu3     = modEntry.Jutsu3;
                    existing.Jutsu4     = modEntry.Jutsu4;
                    existing.Jutsu5     = modEntry.Jutsu5;
                    existing.Jutsu6     = modEntry.Jutsu6;
                    existing.Jutsu1_awa = modEntry.Jutsu1_awa;
                    existing.Jutsu2_awa = modEntry.Jutsu2_awa;
                }
                else
                    SupportSkillRecoverySpeedParamList.Add((SupportSkillRecoverySpeedParamModel)modEntry.Clone());
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
            fileBytes = b_AddString(fileBytes, "bin_le/x64/supportSkillRecoverySpeedParam.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "supportSkillRecoverySpeedParam");
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

            fileBytes = b_AddBytes(fileBytes, new byte[40]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x79,0x48,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x79,0x48,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,
                0x00,0x79,0x5C,0x00,0x00,0x00,0x00,0x00
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x04;
            int countIndex = fileBytes.Length + 0x04;

            fileBytes = b_AddBytes(fileBytes, new byte[16]
            { 0xE9,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00 });

            int startPtr = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[SupportSkillRecoverySpeedParamList.Count * 0x24]);

            for (int x = 0; x < SupportSkillRecoverySpeedParamList.Count; x++)
            {
                int ptr = startPtr + (x * 0x24);
                var e = SupportSkillRecoverySpeedParamList[x];
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.CharacodeID), ptr);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu1),      ptr + 0x04);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu2),      ptr + 0x08);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu3),      ptr + 0x0C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu4),      ptr + 0x10);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu5),      ptr + 0x14);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu6),      ptr + 0x18);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu1_awa),  ptr + 0x1C);
                // BUG ASLI: offset 0x20 juga tulis Jutsu1_awa (bukan Jutsu2_awa)
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(e.Jutsu1_awa),  ptr + 0x20);
            }

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((SupportSkillRecoverySpeedParamList.Count * 0x24) + 0x14), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((SupportSkillRecoverySpeedParamList.Count * 0x24) + 0x10), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(SupportSkillRecoverySpeedParamList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x79,0x8D,0x77,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
