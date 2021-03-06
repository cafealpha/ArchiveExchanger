﻿using System;
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
        public EventCompressArgs(int data)
        {
            compressProg = data;
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
        
        //public string filename;

        FileInfo fi;
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
        //압축한 파일 개수
        public int compressFileCount;


        public zipManager(string filename)
        {
            SevenZipBase.SetLibraryPath(Directory.GetCurrentDirectory() + @"\7z.dll");
            init();

            fi = new FileInfo(filename);
        }

        public void init()
        {
            stDic.Clear();
            totalFileCount = 0;
            extractFileCount = 0;
            compressFileCount = 0;
            origFilename = "";
        }

        //압축풀기
        public void extractFiles(string path)
        {
            sze = new SevenZipExtractor(fi.FullName);
            sze.ExtractionFinished += ExtractFinished;
            //압축풀 총 파일 개수
            var items = sze.ArchiveFileData.Where(item => !item.IsDirectory);
            totalFileCount = items.Count();

            if (sze.ArchiveFileData.Sum(t => (int)t.Size) < 100000000)
            {
                foreach (var item in items)
                {
                    MemoryStream st = new MemoryStream();
                    sze.ExtractFile(item.FileName, st);
                    st.Position = 0;
                    stDic.Add(item.FileName, st);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    FileStream fs = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate);
                    sze.ExtractFile(item.FileName, fs);
                    fs.Position = 0;
                    stDic.Add(item.FileName, fs);
                }

                //후처리
                //foreach(FileStream fs in stDic.Values)
                //{
                //    string temp = fs.Name;
                //    fs.Close();
                //    File.Delete(temp);
                //}
                //stDic.Clear();
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
            szc.FileCompressionFinished += compressingProg;
            //출력할 포멧
            if (format.ToLower() == "zip")
                szc.ArchiveFormat = OutArchiveFormat.Zip;
            else if (format.ToLower() == "7z")
                szc.ArchiveFormat = OutArchiveFormat.SevenZip;
            else
            {
                return;
            }
            //확실하게 파일 닫을 수 있도록
            //using (FileStream fs = new FileStream(fi.FullName.Replace(".part1", "").Replace(fi.Extension, "." + format.ToLower()), FileMode.OpenOrCreate))
            //{
            //    szc.CompressStreamDictionary(stDic, fs);
            //    //szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + format.ToLower()));
            //}
            szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + format.ToLower()));
            //후처리
            if (sze.ArchiveFileData.Sum(t => (int)t.Size) > 100000000)
            {
                foreach (FileStream fs in stDic.Values)
                {
                    string temp = fs.Name;
                    fs.Close();
                    File.Delete(temp);
                }
            }
            stDic.Clear();
        }

        private void compressingProg(object sender, EventArgs e)
        {
            compressFileCount++;
            compressing(this, new EventCompressArgs(
                (int)((compressFileCount / (float)totalFileCount) * 100)
                ));
        }


        //private void compressingProg(object sender, ProgressEventArgs e)
        //{
        //    compressing(this, new EventCompressArgs(e.PercentDone));
        //}
    }
}
