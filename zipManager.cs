using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;
using System.IO;


namespace archiveExchanger
{
    class zipManager
    {
        public string filename;

        //압축 해제 기능
        SevenZipExtractor sze;
        //압축기능
        SevenZipCompressor szc;

        //압축 해체 스트림 딕셔너리
        public Dictionary<string, Stream> stDic = new Dictionary<string, Stream>();

        private string origFilename;

        string _destExt;
        public string destExt
        {
            get
            {
                return _destExt;
            }
            set
            {
                _destExt = value;
            }
        }

        public zipManager()
        {
            SevenZipBase.SetLibraryPath(Directory.GetCurrentDirectory() + @"\7z.dll");
        }

        //압축풀기
        public void extractFiles(fileListData file)
        {
            origFilename = file.fullPath;

            sze = new SevenZipExtractor(origFilename);

            foreach (var item in sze.ArchiveFileData.Where(item => !item.IsDirectory))
            {
                MemoryStream st = new MemoryStream();
                sze.ExtractFile(item.FileName, st);
                st.Position = 0;
                stDic.Add(item.FileName, st);
            }
        }

        //압축하기
        public void compressFiles(string format)
        {
            szc = new SevenZipCompressor();
            //출력할 포멧
            if (format.ToLower() == "zip")
                szc.ArchiveFormat = OutArchiveFormat.Zip;
            else if (format.ToLower() == "7z")
                szc.ArchiveFormat = OutArchiveFormat.SevenZip;

            FileInfo fi = new FileInfo(origFilename);

            szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
        }

    }
}
