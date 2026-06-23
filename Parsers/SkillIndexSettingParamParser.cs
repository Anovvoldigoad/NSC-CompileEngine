// Ported from NSC-ModManager/ViewModel/SkillIndexSettingParamViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, dialogs, RelayCommand
// CATATAN: StartOfFile pakai 0x44 (bukan 0x34 seperti parser lain)
//          Header sebelum data: 40 bytes (bukan 28)
//          Struct size: 0x0C per entry
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class SkillIndexSettingParamParser
    {
        public List<SkillIndexSettingParamModel> SkillIndexSettingParamList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => SkillIndexSettingParamList.Clear();

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

            if (!binName.Contains("skillIndexSettingParam"))
                throw new InvalidDataException($"File is not a skillIndexSettingParam xfbin. BinName={binName}");

            // PENTING: offset 0x44 bukan 0x34
            int startOfFile = 0x44 + b_ReadIntRev(FileByte, 16);
            int entryCount = b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 0x10 + (c * 0x0C);
                SkillIndexSettingParamList.Add(new SkillIndexSettingParamModel
                {
                    CharacodeID = b_ReadInt(FileByte, ptr),
                    JutsuIndex1 = b_ReadInt(FileByte, ptr + 0x04),
                    JutsuIndex2 = b_ReadInt(FileByte, ptr + 0x08),
                });
            }
        }

        /// <summary>
        /// Merge mod entries ke vanilla.
        /// Entry dengan CharacodeID sama → overwrite. Entry baru → append.
        /// </summary>
        public void MergeWith(SkillIndexSettingParamParser modParser)
        {
            foreach (var modEntry in modParser.SkillIndexSettingParamList)
            {
                var existing = SkillIndexSettingParamList
                    .FirstOrDefault(e => e.CharacodeID == modEntry.CharacodeID);
                if (existing != null)
                {
                    existing.JutsuIndex1 = modEntry.JutsuIndex1;
                    existing.JutsuIndex2 = modEntry.JutsuIndex2;
                }
                else
                {
                    SkillIndexSettingParamList.Add((SkillIndexSettingParamModel)modEntry.Clone());
                }
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
            fileBytes = b_AddString(fileBytes, "bin_le/x64/skillIndexSettingParam.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "skillIndexSettingParam");
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

            // Header 40 bytes (berbeda dari parser lain yang 28 bytes)
            fileBytes = b_AddBytes(fileBytes, new byte[40]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x79,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x79,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,
                0x00,0x79,0x00,0x00,0x00,0x00,0x00,0x00
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x04;
            int countIndex = fileBytes.Length + 0x04;

            fileBytes = b_AddBytes(fileBytes, new byte[16]
            {
                0xE9,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            });

            int startPtr = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[SkillIndexSettingParamList.Count * 0x0C]);

            for (int x = 0; x < SkillIndexSettingParamList.Count; x++)
            {
                int ptr = startPtr + (x * 0x0C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(SkillIndexSettingParamList[x].CharacodeID), ptr);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(SkillIndexSettingParamList[x].JutsuIndex1), ptr + 0x04);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(SkillIndexSettingParamList[x].JutsuIndex2), ptr + 0x08);
            }

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((SkillIndexSettingParamList.Count * 0xC) + 0x14), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((SkillIndexSettingParamList.Count * 0xC) + 0x10), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(SkillIndexSettingParamList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x79,0x88,0x77,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
