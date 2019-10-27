﻿using Quasar.Controls;
using Quasar.Resources;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static Quasar.Library;
using Quasar.Singleton;
using System.Threading;
using System;
using System.Globalization;
using Quasar.File;
using Quasar.Quasar_Sys;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Quasar
{
    public partial class MainWindow : Window
    {

        public List<Mod> Mods;
        public List<Mod> WorkingModList;
        public List<ModListElement> ListMods { get; set; }

        public Mod SelectedMod { get; set; }

        public QuasarDownloads DLS;

        List<ModType> ModTypes { get; set; }
        List<Character> Characters { get; set; }
        List<Family> Families { get; set; }

        Mutex serverMutex;

        public MainWindow()
        {
            //Setting up Server or Client
            DLS = new QuasarDownloads();
            DLS.List.CollectionChanged += QuasarDownloadCollectionChanged;
            serverMutex = Checker.Instances(serverMutex, DLS.List);

            //Pre-run checks
            bool Update = Checker.checkUpdated();
            Folderino.CheckBaseFolders();
            Folderino.CheckBaseFiles();
            if (Update)
            {
                Folderino.UpdateBaseFiles();
            }

            //Setting language
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            //Aww, here we go again
            InitializeComponent();

            //Loading things
            LoadBasicLists();
            LoadMods();

        }

        #region XML LOAD
        //Load Mod Library into memory
        private void LoadMods()
        {
            Mods = GetModListFile();
            ListMods = new List<ModListElement>();
            WorkingModList = new List<Mod>();

            foreach (Mod x in Mods)
            {
                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.setMod(x);
                mle.Downloaded = true;
                ListMods.Add(mle);
            }

            ModListView.ItemsSource = ListMods;
        }

        private void LoadBasicLists()
        {
            LoadModTypes();
            LoadCharacterList();
            LoadFamilies();

        }

        private void LoadModTypes()
        {
            ModTypes = XML.GetModTypes();
            ModTypeSelect.ItemsSource = ModTypes;
        }

        private void LoadCharacterList()
        {
            Characters = XML.GetCharacters();
        }

        private void LoadFamilies()
        {
            Families = XML.GetFamilies();
        }
        #endregion

        #region INTERFACE ACTIONS

        private void ModSelected(object sender, SelectionChangedEventArgs e)
        {
            ModListElement mle = (ModListElement)ModListView.SelectedItem;
            SelectedMod = mle.LocalMod;
            if (mle.Downloaded)
            {
                PrintModInformation(mle.LocalMod);
            }
            
        }

        //Refreshes the content of the mod list based on mod type
        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ModType selectedType = (ModType)comboBox.SelectedItem;
            FilterList(selectedType.ID);
            ShowAdvancedFilters(selectedType.ID);
        }

        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowAdvancedFilters(int _modType)
        {
            switch (_modType)
            {
                case 0:
                    ModFilterSelect.ItemsSource = Characters;
                    break;
                case 1:
                    ModFilterSelect.ItemsSource = Families;
                    break;
                case 2:
                    ModFilterSelect.ItemsSource = Families;
                    break;
                default:
                    ModFilterSelect.ItemsSource = null;
                    break;
            }
        }
        private void FilterList(int _modType)
        {
            foreach (ModListElement mle in ModListView.Items)
            {
                if (_modType == -1)
                {
                    mle.Visibility = Visibility.Visible;
                }
                else
                {
                    if (mle.modType != _modType)
                    {
                        mle.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        mle.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        //Refreshes the contents of the filter combobox
        public void PrintModInformation(Mod _item)
        {
            //Thrashing the place
            ModInfoStackPanelValues.Children.Clear();
            VersionStackPanel.Children.Clear();

            //Showing Name and Authors
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.Name });
            foreach (String[] author in _item.Authors)
            {
                ModInfoStackPanelValues.Children.Add(new Label() { Content = "- " + author[0] });
            }

            //Showing Version info
            VersionStackPanel.Children.Add(new Label() { Content = _item.Updates });
            VersionStackPanel.Children.Add(new Label() { Content = "Up to Date" });

            //Loading Tree View
            LoadTreeView(ModFileView, new FileManager(_item).libraryContentPath);
        }

        public void LoadTreeView(System.Windows.Controls.TreeView _tv, string _fp)
        {
            _tv.Items.Clear();
            
            foreach(string s in Directory.GetDirectories(_fp)){
                var rootDirectory = new DirectoryInfo(s);
                _tv.Items.Add(CreateDirectoryNode(rootDirectory));
            }
           
            
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name });

            return directoryNode;
        }

        private void ExpandTree()
        {
            foreach (var item in ModFileView.Items)
            {
                var tvi = item as TreeViewItem;
                if (tvi != null){
                    tvi.ExpandSubtree();
                }
            }
        }

        private void MinimizeTree()
        {
            foreach (var item in ModFileView.Items)
            {
                var tvi = item as TreeViewItem;
                if (tvi != null)
                {
                    tvi.IsExpanded = false;
                }
            }
        }

        private void ModFileMaximize_Click(object sender, RoutedEventArgs e)
        {
            ExpandTree();
        }

        private void ModFileMinimize_Click(object sender, RoutedEventArgs e)
        {
            MinimizeTree();
        }

        #endregion

        #region Settings
        //Deletes Everything Quasar has stored cause that's the easy way out
        private void DeleteDocumentFolderContents(object sender, RoutedEventArgs e)
        {
            Folderino.DeleteDocumentsFolder();
        }

        private void ActivateCustomProtocol(object sender, RoutedEventArgs e)
        {

            if (Protoman.ActivateCustomProtocol())
            {
                Console.WriteLine("Fix Successful");
            }
            else
            {
                Console.WriteLine("You need admin rights to do that");
            }
        }

        #endregion

        #region Downloads
        public class QuasarDownloads
        {
            public ObservableCollection<string> List { get; set; }

            public QuasarDownloads()
            {
                List = new ObservableCollection<string>();
            }
        }

        //Launches a Quasar Download from it's URL
        private async void LaunchDownload(string _URL)
        {
            bool newElement = false;
            string downloadText = "";

            ModListElement mle = new ModListElement();
            FileManager FMan = new FileManager(_URL);

            Mod mod = Mods.Find(mm => mm.id == Int32.Parse(FMan.modID) && mm.type == Int32.Parse(FMan.modType));

            
            //Checking if Mod is already in library
            if(mod != null)
            {
                mle = ListMods.Find(ml => ml.LocalMod == mod);
                downloadText = "Updating mod";
            }
            else
            {
                mod = new Mod(Int32.Parse(FMan.modID), Int32.Parse(FMan.modType), false);
                newElement = true;
                downloadText = "Downloading new mod";
            }
            if (!WorkingModList.Contains(mod))
            {
                WorkingModList.Add(mod);

                if (newElement)
                {
                    //Creating interface element
                    ListMods.Add(mle);
                    ModListView.Items.Refresh();
                }

                //Setting download UI
                mle.Title.Content = downloadText;

                Downloader modDownloader = new Downloader(mle.Progress, mle.Status);

                //Wait for download completion
                await modDownloader.DownloadArchiveAsync(FMan);

                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.getMod(FMan.APIType, FMan.modID);

                //Create Mod from API information
                Mod newmod = Parse(newAPIMod, ModTypes);
                
                if (newElement)
                {
                    //Adding element to list
                    Mods.Add(newmod);
                }
                else
                {
                    //Updating List
                    Mods[Mods.IndexOf(mod)] = newmod;
                }

                //Updating list and saving XML
                WriteModListFile(Mods);


                //Setting extract UI
                mle.Title.Content = "Extracting mod";

                //Preparing Extraction
                Unarchiver un = new Unarchiver(mle.Progress, mle.Status);

                //Wait for Archive extraction
                await un.ExtractArchiveAsync(FMan.downloadDest, FMan.archiveContentDest, FMan.modArchiveFormat);

                //Setting extract UI
                mle.Title.Content = "Moving files";

                //Moving files
                await FMan.MoveDownload();

                //Cleanup
                FMan.ClearDownloadContents();

                //Providing mod to ModListElement and showing info
                mle.setMod(newmod);
                mle.Downloaded = true;

                //Refresh UI element
                mle.refreshUI();
                ModListView.SelectedItem = mle;

                //Removing mod from Working List
                WorkingModList.Remove(mod);
            }
        }

        public void QuasarDownloadCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (string quasari in DLS.List)
            {
                Dispatcher.BeginInvoke((Action)(() => { LaunchDownload(quasari); }));
            }
        }

        #endregion


    }

    
}
