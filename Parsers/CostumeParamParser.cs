// Ported from NSC-ModManager/ViewModel/CostumeParamViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, dialogs, RelayCommand
// CATATAN: StartOfFile pakai 0x44, struct 0x28 per entry
//          EntryString dan CharacterName pakai pointer relatif (bukan offset fixed)
using System.Text;
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class CostumeParamParser
    {
        public List<CostumeParamModel> CostumeParamList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => CostumeParamList.Clear();

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

            if (!binName.Contains("costumeParam"))
                throw new InvalidDataException($"File is not a costumeParam xfbin. BinName={binName}");

            int startOfFile = 0x44 + b_ReadIntRev(FileByte, 16);
            int entryCount = b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 0x10 + (c * 0x28);
                CostumeParamList.Add(new CostumeParamModel
                {
                    EntryString          = b_ReadString(FileByte, ptr + b_ReadInt(FileByte, ptr)),
                    EntryIndex           = b_ReadInt(FileByte, ptr + 0x08),
                    PlayerSettingParamID = b_ReadInt(FileByte, ptr + 0x0C),
                    CharacterName        = b_ReadString(FileByte, ptr + 0x10 + b_ReadInt(FileByte, ptr + 0x10)),
                    EntryType            = b_ReadInt(FileByte, ptr + 0x18),
                    UnlockCost           = b_ReadInt(FileByte, ptr + 0x1C),
                    UnlockCondition      = b_ReadInt(FileByte, ptr + 0x20),
                });
            }
        }

        public void MergeWith(CostumeParamParser modParser)
        {
            foreach (var modEntry in modParser.CostumeParamList)
            {
                var existing = CostumeParamList
                    .FirstOrDefault(e => e.EntryString == modEntry.EntryString);
                if (existing != null)
                {
                    existing.EntryIndex           = modEntry.EntryIndex;
                    existing.PlayerSettingParamID = modEntry.PlayerSettingParamID;
                    existing.CharacterName        = modEntry.CharacterName;
                    existing.EntryType            = modEntry.EntryType;
                    existing.UnlockCost           = modEntry.UnlockCost;
                    existing.UnlockCondition      = modEntry.UnlockCondition;
                }
                else
                    CostumeParamList.Add((CostumeParamModel)modEntry.Clone());
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
            fileBytes = b_AddString(fileBytes, "bin_le/x64/costumeParam.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "costumeParam");
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
            { 0xE8,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00 });

            int startPtr = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[CostumeParamList.Count * 0x28]);

            int addSize = 0;
            var entryStringPtrs   = new List<int>();
            var characterNamePtrs = new List<int>();

            for (int x = 0; x < CostumeParamList.Count; x++)
            {
                int ptr = startPtr + (x * 0x28);

                entryStringPtrs.Add(fileBytes.Length);
                if (!string.IsNullOrEmpty(CostumeParamList[x].EntryString))
                {
                    fileBytes = b_AddBytes(fileBytes, Encoding.ASCII.GetBytes(CostumeParamList[x].EntryString));
                    fileBytes = b_AddBytes(fileBytes, new byte[1]);
                    int newPtr = entryStringPtrs[x] - startPtr - x * 0x28;
                    fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(newPtr), ptr);
                    addSize += CostumeParamList[x].EntryString.Length + 1;
                }

                characterNamePtrs.Add(fileBytes.Length);
                if (!string.IsNullOrEmpty(CostumeParamList[x].CharacterName))
                {
                    fileBytes = b_AddBytes(fileBytes, Encoding.ASCII.GetBytes(CostumeParamList[x].CharacterName));
                    fileBytes = b_AddBytes(fileBytes, new byte[1]);
                    int newPtr = characterNamePtrs[x] - startPtr - x * 0x28 - 0x10;
                    fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(newPtr), ptr + 0x10);
                    addSize += CostumeParamList[x].CharacterName.Length + 1;
                }

                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList[x].EntryIndex),           ptr + 0x08);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList[x].PlayerSettingParamID), ptr + 0x0C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList[x].EntryType),            ptr + 0x18);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList[x].UnlockCost),           ptr + 0x1C);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList[x].UnlockCondition),      ptr + 0x20);
            }

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((CostumeParamList.Count * 0x28) + 0x14 + addSize), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((CostumeParamList.Count * 0x28) + 0x10 + addSize), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(CostumeParamList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x79,0x8D,0x77,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
