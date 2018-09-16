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

        public int progress
        {
            get
            {
                return (_extractPrograss + _compressPrograss) / 2;
            }
            set
            {

            }
        }

        private int _extractPrograss;
        public int extractPrograss
        {
            get
            {
                return _extractPrograss;
            }
            set
            {
                _extractPrograss = value;
                OnPropertyChanged("progress");
            }
        }

        private int _compressPrograss;
        public int compressPrograss
        {
            get
            {
                return _compressPrograss;
            }
            set
            {
                _compressPrograss = value;
                OnPropertyChanged("progress");
            }
        }



        public fileListData(string name, string ext)
        {

            fi = new FileInfo(name);
            
            destExt = ext;

            _destFormat.Add("ZIP");
            _destFormat.Add("7z");
            OnPropertyChanged("destFileName");
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
                return Path.GetFileNameWithoutExtension(fi.Name).Replace(".part1","") + "." + destExt.ToLower();
//                return Path.GetFileNameWithoutExtension(fi.Name) + "." + destExt.ToLower();
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

        public void workingStart()
        {
            zipManager zm = new zipManager(fullPath);
            //이벤트 핸들러 등록
            zm.extracting += new zipManager.extractingEventHandler(extractProg);
            zm.compressing += new zipManager.compressingEventHandler(compressProg);

            zm.extractFiles(fullPath);

            zm.compressFiles(destExt);
            
        }

        private void compressProg(object sender, EventCompressArgs e)
        {
            compressPrograss = e.compressProg;
        }

        private void extractProg(object sender, EventExtractArgs e)
        {
            extractPrograss = e.extractProg;
        }
        //파일 소스의 전체 경로가 같으면 같은 것으로 본다.
        public bool Equals(fileListData other)
        {
            if (fi.FullName == other.fi.FullName)
                return true;

            return false;
        }
    }
}
