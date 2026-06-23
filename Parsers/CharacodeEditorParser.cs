// Ported from NSC-ModManager/ViewModel/CharacodeEditorViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, OpenFileDialog,
//           SaveFileDialog, ModernWpf.MessageBox, CollectionViewSource, RelayCommand
// Logic: 100% identical to original
using System.Text;
using NSC_CompileEngine.Models;
using static NSC_CompileEngine.BinaryReader;

namespace NSC_CompileEngine.Parsers
{
    public class CharacodeEditorParser
    {
        public List<CharacodeEditorModel> CharacodeList { get; private set; } = new();
        public string FilePath { get; private set; } = "";

        public void Clear() => CharacodeList.Clear();

        public void OpenFile(string path)
        {
            Clear();
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            FilePath = path;
            byte[] fileBytes = File.ReadAllBytes(path);
            int index = 128;
            string binPath = b_ReadString(fileBytes, index);
            index += binPath.Length + 2;

            string binName = "";
            for (int x = 0; x < 3; x++)
            {
                string name = b_ReadString(fileBytes, index);
                if (x == 0) binName = name;
                index += name.Length + 1;
            }

            if (!binName.Contains("characode"))
                throw new InvalidDataException($"File is not a characode xfbin. BinName={binName}");

            int startOfFile = 0x34 + b_ReadIntRev(fileBytes, 16);
            int entryCount = b_ReadInt(fileBytes, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 8 + (c * 0x08);
                CharacodeList.Add(new CharacodeEditorModel
                {
                    CharacodeIndex = c + 1,
                    CharacodeName = b_ReadString(fileBytes, ptr)
                });
            }
        }

        public void MergeWith(CharacodeEditorParser modParser)
        {
            foreach (var modEntry in modParser.CharacodeList)
            {
                var existing = CharacodeList.FirstOrDefault(c =>
                    c.CharacodeName == modEntry.CharacodeName);
                if (existing == null)
                {
                    // Entry baru — tambah
                    CharacodeList.Add(new CharacodeEditorModel
                    {
                        CharacodeIndex = CharacodeList.Count + 1,
                        CharacodeName = modEntry.CharacodeName
                    });
                }
                // Kalau sudah ada, skip (characode sudah unik per nama)
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
                0x4E,0x55,0x43,0x43,0x00,0x00,0x00,0x63,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x80,0xBC,0x00,0x00,0x00,0x03,0x00,0x63,0x40,0x00,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x3B,0x00,0x00,0x01,0x49,0x00,0x00,0x4C,0xE3,0x00,0x00,0x01,0x4B,
                0x00,0x00,0x0F,0x6F,0x00,0x00,0x01,0x4B,0x00,0x00,0x0F,0x84,0x00,0x00,0x05,0x20,
                0x00,0x00,0x00,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x4E,0x75,0x6C,
                0x6C,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x42,0x69,0x6E,0x61,0x72,
                0x79,0x00,0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x50,0x61,0x67,0x65,0x00,
                0x6E,0x75,0x63,0x63,0x43,0x68,0x75,0x6E,0x6B,0x49,0x6E,0x64,0x65,0x78,0x00
            };

            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "D:/next5/char_hi/param/player/Converter/bin/characode.bin");
            fileBytes = b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[1]);
            fileBytes = b_AddString(fileBytes, "characode");
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

            int totalLength = fileBytes.Length;
            int pathLength = ptrPath - 127;
            int nameLength = ptrName - ptrPath;
            int section1Length = ptrSection - ptrName - addedBytes;
            int fullLength = totalLength - 68 + 40;

            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(fullLength), 16, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(2), 36, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(pathLength), 40, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 44, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(nameLength), 48, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 52, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(section1Length), 56, 1);
            fileBytes = b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 60, 1);

            fileBytes = b_AddBytes(fileBytes, new byte[28]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x98,
                0x00,0x00,0x00,0x01,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x94
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x4;
            int countIndex = fileBytes.Length;
            fileBytes = b_AddBytes(fileBytes, new byte[4]);
            int startOfFile = fileBytes.Length;

            fileBytes = b_AddBytes(fileBytes, new byte[8 * CharacodeList.Count]);
            for (int c = 0; c < CharacodeList.Count; c++)
            {
                if (string.IsNullOrEmpty(CharacodeList[c].CharacodeName))
                    CharacodeList[c].CharacodeName = "x002";
                int ptr = startOfFile + (c * 8);
                fileBytes = b_ReplaceBytes(fileBytes,
                    Encoding.ASCII.GetBytes(CharacodeList[c].CharacodeName), ptr);
            }

            fileBytes = b_ReplaceBytes(fileBytes,
                BitConverter.GetBytes((CharacodeList.Count * 8) + 8), size1Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes,
                BitConverter.GetBytes((CharacodeList.Count * 8) + 4), size2Index, 1);
            fileBytes = b_ReplaceBytes(fileBytes,
                BitConverter.GetBytes(CharacodeList.Count), countIndex);

            return b_AddBytes(fileBytes, new byte[20]
            {
                0,0,0,8,0,0,0,2,0,99,0,0,0,0,0,4,0,0,0,0
            });
        }
    }
}
