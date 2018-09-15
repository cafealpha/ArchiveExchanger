using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

//폴더 선택을 위해서 윈폼 컨트롤 사용
using winForm = System.Windows.Forms;

namespace archiveExchanger
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public int maxThread;

        private ObservableCollection<fileListData> _items = new ObservableCollection<fileListData>();
        public ObservableCollection<fileListData> Items
        {
            get
            {
                return _items;
            }
        }
        
        //체크박스들을 담을 리스트
        //private List<CheckBox> _cbList = new List<CheckBox>();
        public List<CheckBox> cbList
        {
            get
            {
                List<CheckBox> _cbList = new List<CheckBox>();
                if (sbCheck == null) return _cbList;
                _cbList.Clear();
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    foreach (CheckBox cb in sbCheck.Children.OfType<CheckBox>())
                    {
                        _cbList.Add(cb);
                    }
                }));
                return _cbList;
            }
        }

        //콤보박스에 선택된 항목을 반환
        public string destExt
        {
            get
            {
                string result = "";
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                {
                    result = (cbDestFormat.SelectedItem as ComboBoxItem).Content as string;
                }));
                return result;
            }
        }

        //체크박스에 체크된 확장자들을 가져와 비교할 리스트를 만든다
        List<string> _checkedExtList = new List<string>();
        List<string> checkedExtList
        {
            get
            {
                _checkedExtList.Clear();
                foreach (var ext in cbList)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        if (ext.IsChecked == true)
                            _checkedExtList.Add((ext.Content as string).ToLower());
                    }));
                }
                return _checkedExtList;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Init();

            //리스트 테스트
            //Items.Add(new fileListData(cbList[0].Content as string));
        }

        public void Init()
        {
            //쓰레드풀 개수를 4개로 제한
            maxThread = 4;
            ThreadPool.SetMaxThreads(maxThread, maxThread);

            //디버그용 텍스트박스에 자동으로 스크롤되도록
            tbDebugBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }


        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            string targetDir = selectDir();
            if (targetDir == null) return;
            Thread t1 = new Thread(() => SearchDir(targetDir, checkedExtList, SearchOption.AllDirectories));
            t1.Start();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            //파일 브라우져 오픈
            string[] targetFiles = selectFile();
            if (targetFiles == null) return;

            insertFiles(targetFiles);

            //Thread t1 = new Thread(() => SearchDir(fd.SelectedPath, checkedExtList, SearchOption.AllDirectories));
            //t1.Start();
        }
        
        //파일 브라우져 오픈해주는 매서드
        private string[] selectFile()
        {
            Microsoft.Win32.OpenFileDialog fd = new Microsoft.Win32.OpenFileDialog();
            fd.Multiselect = true;
            if (fd.ShowDialog() == false)
                return null;

            List<string> result = new List<string>();

            //LINKQ를 사용해 파일리스트 중 찾고자 하는 확장자를 가지는 파일을 추출
            //비교를 위해 일괄적으로 소문자로 변환후에 찾는다.
            if (cbList == null) return null;
            foreach(var file in fd.FileNames)
            {
                var source = System.IO.Path.GetExtension(file).ToLower().Replace(".", "");
                foreach (var cb in cbList)
                {
                    //getExtension이 반환하는 값이 .을 포함하기 때문에 비교가 안됨.
                    var compare  = (cb.Content as string).ToLower();
                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        if (source == compare)
                        {
                            result.Add(file);
                        }
                    }));
                }
            }

            return result.ToArray();
        }

        private string selectDir()
        {
            winForm.FolderBrowserDialog fd = new winForm.FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return null;

            return fd.SelectedPath;
        }

        public void SearchDir(string target, List<string> ext, SearchOption so)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                btnAddFolder.IsEnabled = false;
            }));

            //LINKQ를 사용해 파일리스트 중 찾고자 하는 확장자를 가지는 파일을 추출
            //비교를 위해 일괄적으로 소문자로 변환후에 찾는다.
            var files = from file in Directory.EnumerateFiles(target, "*.*", so)
                        //where searchTerm.Matches(file).Count > 0
                    where checkedExtList.Any(System.IO.Path.GetExtension(file).ToLower().Contains)
                    select file;

            insertFiles(files);

            Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                btnAddFolder.IsEnabled = true;
            }));
        }
        
        //Items에 중복확인하고 아이템 삽입해주는 메소드
        private void insertFiles(IEnumerable<string> files)
        {
            //분할 예정

                foreach (var file in files)
                {
                    //현재 지정되어있는 타겟압축파일과 같으면 넣지 않음.
                    if (System.IO.Path.GetExtension(file).ToLower().Replace(".", "") == destExt.ToLower()) continue;

                    var tf = new fileListData(file, destExt);
                    if (Items.Contains(tf))
                        continue;

                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                    {
                        Items.Add(tf);
                    }));

                    Thread.Sleep(1);
                }

        }

        private void cbDestFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //콤보박스에서 선택된 아이템의 이름
            string selectedItemName = (e.AddedItems[0] as ComboBoxItem).Content as string;


            Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            {
                if (tbDebugBox != null)
                    tbDebugBox.Text = selectedItemName;

                if (cbList == null) return;
                foreach(var cb in cbList)
                {
                    //체크되어있지 않으면 필요없다.
                    if (cb.IsChecked == false) continue;
                    if(cb.Content as string == selectedItemName)
                    {
                        cb.IsChecked = false;
                    }
                }
            }));
        }

        private void lvFileList_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.Key == Key.Delete)
            {
                if (lvFileList.SelectedItems.Count == 0) return;

                List<fileListData> list = new List<fileListData>();

                foreach (fileListData item in lvFileList.SelectedItems)
                {
                    list.Add(item);
                }

                foreach(var item in list)
                {
                    Items.Remove(item);
                }

            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void btnConvertStart_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(startConvert, Items[0]);
        }

        private void startConvert(object obj)
        {
            (obj as fileListData).workingStart();
        }
    }
}
