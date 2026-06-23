// Ported from NSC-ModManager/ViewModel/EffectPrmViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, OpenFileDialog,
//           SaveFileDialog, ModernWpf.MessageBox, CollectionViewSource, RelayCommand
// Logic: 100% identical to original — struct size 0x88 per entry
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class EffectPrmParser
    {
        public List<EffectPrmModel> EffectPrmList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => EffectPrmList.Clear();

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

            if (!binName.Contains("effectprm"))
                throw new InvalidDataException($"File is not an effectprm xfbin. BinName={binName}");

            int startOfFile = 0x34 + b_ReadIntRev(FileByte, 16);
            int entryCount = b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 8 + (c * 0x88);
                EffectPrmList.Add(new EffectPrmModel
                {
                    EffectPrmID = b_ReadInt(FileByte, ptr),
                    Type        = b_ReadInt(FileByte, ptr + 0x04),
                    FilePath    = b_ReadString(FileByte, ptr + 0x08),
                    ChunkName   = b_ReadString(FileByte, ptr + 0x48),
                });
            }
        }

        /// <summary>
        /// Merge mod entries ke vanilla list.
        /// Entry dengan EffectPrmID sama → overwrite. Entry baru → append.
        /// </summary>
        public void MergeWith(EffectPrmParser modParser)
        {
            foreach (var modEntry in modParser.EffectPrmList)
            {
                var existing = EffectPrmList.FirstOrDefault(e =>
                    e.EffectPrmID == modEntry.EffectPrmID);
                if (existing != null)
                {
                    existing.Type      = modEntry.Type;
                    existing.FilePath  = modEntry.FilePath;
                    existing.ChunkName = modEntry.ChunkName;
                }
                else
                {
                    EffectPrmList.Add((EffectPrmModel)modEntry.Clone());
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
            fileBytes = b_AddString(fileBytes, "D:/next5/char_hi/param/player/Converter/bin/effectprm.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "effectprm");
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

            int pathLength    = ptrPath - 127;
            int nameLength    = ptrName - ptrPath;
            int section1Len   = ptrSection - ptrName - addedBytes;
            int fullLength    = fileBytes.Length - 68 + 40;

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(fullLength),   16, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(2),            36, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(pathLength),   40, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),            44, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(nameLength),   48, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),            52, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(section1Len),  56, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4),            60, 1);

            fileBytes = b_AddBytes(fileBytes, new byte[28]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x98,
                0x00,0x00,0x00,0x01,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x94
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x04;
            int countIndex = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[4]);
            int startOfFile = fileBytes.Length;

            fileBytes = b_AddBytes(fileBytes, new byte[EffectPrmList.Count * 0x88]);
            for (int x = 0; x < EffectPrmList.Count; x++)
            {
                int ptr = startOfFile + (x * 0x88);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(EffectPrmList[x].EffectPrmID), ptr);
                fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(EffectPrmList[x].Type), ptr + 0x04);
                fileBytes = b_ReplaceString(fileBytes, EffectPrmList[x].FilePath,  ptr + 0x08);
                fileBytes = b_ReplaceString(fileBytes, EffectPrmList[x].ChunkName, ptr + 0x48);
            }

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((EffectPrmList.Count * 0x88) + 8), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes((EffectPrmList.Count * 0x88) + 4), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(EffectPrmList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x79,0x8D,0x77,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
