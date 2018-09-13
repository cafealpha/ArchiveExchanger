using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using SevenZip;

namespace archiveExchanger
{
    public class fileListData : INotifyPropertyChanged, IEquatable<fileListData>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //파일 정보 클래스
        public FileInfo fi;
        
        //압축 해제 기능
        SevenZipExtractor sze;
        //압축기능
        SevenZipCompressor szc;

        //압축 해체 스트림 딕셔너리
        public Dictionary<string, Stream> stDic = new Dictionary<string, Stream>();


        //압축파일 총 파일 개수
        private int totalFileCount;
        //카운터
        private int counter;

        //작업 완료 플래그
        private bool _workDone;
        public bool workDone
        {
            get
            {
                return _workDone;
            }
            set
            {
                _workDone = value;
            }
        }

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
                OnPropertyChanged("destFileName");
            }
        }

        public fileListData(string name, string ext)
        {
            SevenZipExtractor.SetLibraryPath(Directory.GetCurrentDirectory() + @"\7z.dll");

            fi = new FileInfo(name);
            
            destExt = ext;

            counter = 0;

            _destFormat.Add("ZIP");
            _destFormat.Add("7z");
            OnPropertyChanged("destFileName");

            //객체 생성
            sze = new SevenZipExtractor(fi.FullName);
            szc = new SevenZipCompressor();

            //이벤트 연결
            sze.ExtractionFinished += extractCount;
            szc.Compressing += compressEvent;

            totalFileCount = (from file in sze.ArchiveFileData
                              where !file.IsDirectory
                              select file).Count();


        }

        private void compressEvent(object sender, ProgressEventArgs e)
        {
            sze.ArchiveFileData.Where(file => !file.IsDirectory).Count();
        }

        private void extractCount(object sender, EventArgs e)
        {
        }

        public string fullPath
        {
            get { return fi.FullName; }
        }


        public string sourceFileName
        {
            get
            {
                return fi.Name;
            }
        }

        public string destFileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(fi.Name) + "." + destExt.ToLower();
            }
        }


        public string origin
        {
            get
            {
                return fi.DirectoryName;
            }
        }

        private ObservableCollection<string> _destFormat = new ObservableCollection<string>();
        public ObservableCollection<string> destFormat
        {
            get
            {
                return _destFormat;
            }
        }

        //압축풀기
        public void extractFiles()
        {
            foreach (var file in sze.ArchiveFileData.Where(file => !file.IsDirectory))
            {
                MemoryStream st = new MemoryStream();
                sze.ExtractFile(file.FileName, st);
                stDic.Add(file.FileName, st);
            }
        }
        //압축하기
        public void compressFiles()
        {
            //출력할 포멧
            if (destExt == "ZIP")
                szc.ArchiveFormat = OutArchiveFormat.Zip;
            else if (destExt == "7z")
                szc.ArchiveFormat = OutArchiveFormat.SevenZip;

            //딕셔너리 파일에서 전체를 압축하므로 의미가 없다.

            ////파일이 존재하면 압축 시작중이라는 이야기이므로 compress모드를 바꿈.
            //if (File.Exists(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower())))
            //    szc.CompressionMode = CompressionMode.Append;
            //else
            //    szc.CompressionMode = CompressionMode.Create;

            //압축 시작
            szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
        }

        //전체
        public void compressConvert()
        {
            //압축풀기
            extractFiles();
            //압축하기 전에 파일이 있으면 삭제
            if (File.Exists(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower())))
            {
                File.Delete(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
            }
            //압축하기
            compressFiles();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                return;
            }

            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs(name);
                PropertyChanged(this, arg);
            }

        }

        //파일 소스의 전체 경로가 같으면 같은 것으로 본다.
        public bool Equals(fileListData other)
        {
            if (fi.FullName == other.fi.FullName)
                return true;

            return false;
        }

        


        ////파일 소스의 전체 경로가 같으면 같은 것으로 본다.
        //public bool Equals(string fullName)
        //{
        //    if (fi.FullName == Path.GetFullPath(fullName))
        //        return true;

        //    return false;
        //}
    }
}
