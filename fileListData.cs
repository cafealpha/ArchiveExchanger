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

        private int _progress;
        public int progress
        {
            get
            {
                return _progress;        
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
                if(fi.Name.Contains(".part1"))
                {
                    return Path.GetFileNameWithoutExtension(fi.Name).Replace(".part1","") + "." + destExt.ToLower();
                }

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
            zm.extracting += new zipManager.extractingEventHandler(extractProg);
            zm.extractFiles(fullPath);
        }

        private void extractProg(object sender, EventExtractArgs e)
        {
                progress = e.extractProg;
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
