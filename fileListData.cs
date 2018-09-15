using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace archiveExchanger
{
    public class fileListData : INotifyPropertyChanged, IEquatable<fileListData>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //파일 정보 클래스
        public FileInfo fi;
        
        //압축파일 총 파일 개수
        private int totalFileCount;
        //카운터

        //압축 해제 카운터
        private int extractCounter;
        private int compressProgress;

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

        private int _progress;
        public int progress
        {
            get
            {
                return _progress;        
                //return (int)((extractCounter / (float)totalFileCount * 100) + compressProgress);
            }
            set
            {
                _progress = value;
                OnPropertyChanged("progress");
            }
        }

        public fileListData(string name, string ext)
        {

            fi = new FileInfo(name);
            
            destExt = ext;

            extractCounter = 0;

            _destFormat.Add("ZIP");
            _destFormat.Add("7z");
            OnPropertyChanged("destFileName");

            //객체 생성

            //압축파일내 파일개수 카운팅
            //totalFileCount = sze.ArchiveFileData.Where(file => !file.IsDirectory).Count();

            //이벤트 연결
            //sze.ExtractionFinished += extractCount;
            //szc.Compressing += compressEvent;
        }

        //private void compressEvent(object sender, ProgressEventArgs e)
        //{
        //    compressProgress = e.PercentDone;
        //}

        //private void extractCount(object sender, EventArgs e)
        //{
        //    extractCounter++;
        //    OnPropertyChanged("progress");
        //}

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

        ////압축풀기
        //public void extractFiles()
        //{
        //    foreach (var file in sze.ArchiveFileData.Where(file => !file.IsDirectory))
        //    {
        //        MemoryStream st = new MemoryStream();
        //        sze.ExtractFile(file.FileName, st);
        //        st.Position = 0;
        //        stDic.Add(file.FileName, st);
        //    }
        //}
        ////압축하기
        //public void compressFiles()
        //{
        //    //출력할 포멧
        //    if (destExt == "ZIP")
        //        szc.ArchiveFormat = OutArchiveFormat.Zip;
        //    else if (destExt == "7z")
        //        szc.ArchiveFormat = OutArchiveFormat.SevenZip;

        //    //딕셔너리 파일에서 전체를 압축하므로 의미가 없다.

        //    ////파일이 존재하면 압축 시작중이라는 이야기이므로 compress모드를 바꿈.
        //    //if (File.Exists(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower())))
        //    //    szc.CompressionMode = CompressionMode.Append;
        //    //else
        //    //    szc.CompressionMode = CompressionMode.Create;

        //    //압축 시작
        //    szc.CompressStreamDictionary(stDic, fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
        //}

        ////전체
        //public void convertStart()
        //{
        //    //압축풀기
        //    extractFiles();
        //    //압축하기 전에 파일이 있으면 삭제
        //    if (File.Exists(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower())))
        //    {
        //        File.Delete(fi.FullName.Replace(fi.Extension, "." + destExt.ToLower()));
        //    }
        //    //압축하기
        //    compressFiles();
        //}

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
