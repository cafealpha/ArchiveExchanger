using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZip;
using System.IO;


namespace archiveExchanger
{

    public class EventCompressArgs : EventArgs
    {
        //압축 진행상황
        public int compressProg;
        public string filename;
        public EventCompressArgs(int data, string _filename)
        {
            compressProg = data;
            filename = _filename;
        }
    }

    public class EventExtractArgs : EventArgs
    {
        //압축 해제 진행상황 
        //압축 진행상황
        public int extractProg;
        public string filename;
        public EventExtractArgs(int data, string _filename)
        {
            extractProg = data;
            filename = _filename;
        }
    }

    //public delegate void EventHandler(object sender, EventArgs e);
    class zipManager
    {
        public delegate void extractingEventHandler(object sender, EventExtractArgs e);
        public delegate void compressingEventHandler(object sender, EventCompressArgs e);

        public event extractingEventHandler extracting;
        public event compressingEventHandler compressing;
        
        public string filename;

        //압축 해제 기능
        SevenZipExtractor sze;
        //압축기능
        SevenZipCompressor szc;

        //압축 해체 스트림 딕셔너리
        public Dictionary<string, Stream> stDic = new Dictionary<string, Stream>();

        private string origFilename;

        //총 파일 개수
        public int totalFileCount;
        //압축해제한 파일 개수
        public int extractFileCount;

        public int _compressProg;

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
            init();
        }

        public void init()
        {
            stDic.Clear();
            totalFileCount = 0;
            extractFileCount = 0;
            origFilename = "";
            filename = "";
        }

        //압축풀기
        public void extractFiles(string path)
        {
            origFilename = path;

            sze = new SevenZipExtractor(origFilename);
            sze.ExtractionFinished += ExtractFinished;
            //압축풀 총 파일 개수
            var items = sze.ArchiveFileData.Where(item => !item.IsDirectory);
            totalFileCount = items.Count();

            //if (sze.ArchiveFileData.Sum(t => (int)t.Size) < 50000000)
            
            foreach (var item in items)
            {
                //if(item.Size < 5000)
                //{
                    MemoryStream st = new MemoryStream();
                    sze.ExtractFile(item.FileName, st);
                    st.Position = 0;
                    stDic.Add(item.FileName, st);
                //}
                //else
                //{
                //    FileStream fs = new FileStream()
                //}

                
            }
            

        }

        private void ExtractFinished(object sender, EventArgs e)
        {
            extractFileCount++;
            var e2 = new EventExtractArgs((int)((extractFileCount / (float)totalFileCount) * 100), origFilename);
            extracting(this, e2);
        }

        //압축하기
        public void compressFiles(string format)
        {
            szc = new SevenZipCompressor();
            szc.Compressing += compressingProg;

            //출력할 포멧
            if (format.ToLower() == "zip")
                szc.ArchiveFormat = OutArchiveFormat.Zip;
            else if (format.ToLower() == "7z")
                szc.ArchiveFormat = OutArchiveFormat.SevenZip;
            else
            {
                return;
            }
            FileInfo fi = new FileInfo(origFilename);

            szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
        }

        private void compressingProg(object sender, ProgressEventArgs e)
        {
            //compressing(this, new EventCompressArgs { compressProg = e.PercentDelta });

            //compressProg = e.PercentDelta;
        }
    }
}
