// Ported from NSC-ModManager/ViewModel/DamagePrmViewModel.cs
// Stripped: INotifyPropertyChanged, ObservableCollection→List, OpenFileDialog,
//           SaveFileDialog, ModernWpf.MessageBox → exceptions/return values
// Logic: 100% identical to original — offset math, byte layout, header template unchanged.
using NSC_CompileEngine.Models;

namespace NSC_CompileEngine.Parsers
{
    public class DamagePrmParser
    {
        public List<DamagePrmModel> DamagePrmList { get; private set; } = new();
        public byte[] FileByte { get; private set; } = Array.Empty<byte>();
        public string FilePath { get; private set; } = "";

        public void Clear() => DamagePrmList.Clear();

        /// <summary>
        /// Load dan parse file damageprm.xfbin.
        /// Throws jika file tidak ditemukan atau bukan file damageprm.
        /// </summary>
        public void OpenFile(string path)
        {
            Clear();
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            FilePath = path;
            FileByte = File.ReadAllBytes(path);

            int index = 128;
            string binPath = BinaryHelper.b_ReadString(FileByte, index);
            index += binPath.Length + 2;

            string binName = "";
            for (int x = 0; x < 3; x++)
            {
                string name = BinaryHelper.b_ReadString(FileByte, index);
                if (x == 0) binName = name;
                index += name.Length + 1;
            }

            if (!binName.Contains("damageprm"))
                throw new InvalidDataException($"File is not a damageprm xfbin. BinName={binName}");

            int startOfFile = 0x34 + BinaryHelper.b_ReadIntRev(FileByte, 16);
            int entryCount = BinaryHelper.b_ReadInt(FileByte, startOfFile + 4);

            for (int c = 0; c < entryCount; c++)
            {
                int ptr = startOfFile + 8 + (c * 0xA0);
                DamagePrmList.Add(new DamagePrmModel
                {
                    Data = BinaryHelper.b_ReadByteArray(FileByte, ptr, 0xA0)
                });
            }
        }

        /// <summary>
        /// Merge data dari mod file ke vanilla file.
        /// Entry yang ada di mod menggantikan entry vanilla di index yang sama.
        /// </summary>
        public void MergeWith(DamagePrmParser modParser)
        {
            for (int i = 0; i < modParser.DamagePrmList.Count && i < DamagePrmList.Count; i++)
                DamagePrmList[i] = (DamagePrmModel)modParser.DamagePrmList[i].Clone();
        }

        /// <summary>
        /// Simpan ke path yang sama dengan backup otomatis.
        /// </summary>
        public void SaveFile()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                throw new InvalidOperationException("No file path set. Use SaveFileTo() instead.");

            string backup = FilePath + ".backup";
            if (File.Exists(backup)) File.Delete(backup);
            File.Copy(FilePath, backup);
            File.WriteAllBytes(FilePath, ConvertToFile());
        }

        /// <summary>
        /// Simpan ke path tertentu.
        /// </summary>
        public void SaveFileTo(string outputPath)
        {
            FilePath = outputPath;
            File.WriteAllBytes(outputPath, ConvertToFile());
        }

        /// <summary>
        /// Rebuild seluruh file xfbin dari data DamagePrmList.
        /// Logic identik dengan ConvertToFile() di original ViewModel.
        /// </summary>
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

            int ptrNucc = fileBytes.Length;
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);
            fileBytes = BinaryHelper.b_AddString(fileBytes, "D:/next5/branches/masterV110/char_hi/param/player/Converter/bin/damageprm.bin");
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);

            int ptrPath = fileBytes.Length;
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);
            fileBytes = BinaryHelper.b_AddString(fileBytes, "damageprm");
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);
            fileBytes = BinaryHelper.b_AddString(fileBytes, "Page0");
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);
            fileBytes = BinaryHelper.b_AddString(fileBytes, "index");
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]);

            int ptrName = fileBytes.Length;
            int addedBytes = 0;
            while (fileBytes.Length % 4 != 0) { addedBytes++; fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[1]); }

            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[48]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,
                0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03
            });

            int ptrSection = fileBytes.Length;
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[16]
            { 0,0,0,0,0,0,0,1,0,0,0,2,0,0,0,3 });

            int totalLength = fileBytes.Length;
            int pathLength = ptrPath - 127;
            int nameLength = ptrName - ptrPath;
            int section1Length = ptrSection - ptrName - addedBytes;
            int fullLength = totalLength - 68 + 40;

            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(fullLength), 16, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(2), 36, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(pathLength), 40, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 44, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(nameLength), 48, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 52, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(section1Length), 56, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(4), 60, 1);

            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[28]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x98,
                0x00,0x00,0x00,0x01,0x00,0x63,0x79,0x76,0x00,0x00,0x08,0x94
            });

            int size1Index = fileBytes.Length - 0x10;
            int size2Index = fileBytes.Length - 0x4;
            int countIndex = fileBytes.Length;
            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[4]);
            int startOfFile = fileBytes.Length;

            fileBytes = BinaryHelper.b_AddBytes(fileBytes, new byte[DamagePrmList.Count * 0xA0]);
            for (int x = 0; x < DamagePrmList.Count; x++)
            {
                int ptr = startOfFile + (x * 0xA0);
                fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, DamagePrmList[x].Data, ptr);
            }

            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((DamagePrmList.Count * 0xA0) + 0x8), size1Index, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((DamagePrmList.Count * 0xA0) + 0x4), size2Index, 1);
            fileBytes = BinaryHelper.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(DamagePrmList.Count), countIndex);

            return BinaryHelper.b_AddBytes(fileBytes, new byte[20]
            {
                0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x02,0x00,0x79,0x8D,0x77,0x00,0x00,0x00,0x04,
                0x00,0x00,0x00,0x00
            });
        }
    }
}
