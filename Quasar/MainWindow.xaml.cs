﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using Quasar.Controls;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using static Quasar.XMLResources.WorkspaceXML;
using System.Windows.Input;
using Point = System.Windows.Point;
using Quasar.Internal.FileSystem;
using FluentFTP;
using System.Text.RegularExpressions;
using System.Net;
using Quasar.Internal.Tools;

namespace Quasar
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        protected bool m_IsDraging = false;
        protected Point _dragStartPoint;

        #region Triggers
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //Interface Triggers
        private bool _GameSelectOverlayDisplay;
        public bool GameSelectOverlayDisplay
        {
            get
            {
                return _GameSelectOverlayDisplay;
            }
            set
            {
                _GameSelectOverlayDisplay = value;
                GameSelectVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _GameSelectVisibility;
        public Visibility GameSelectVisibility
        {
            get
            {
                return _GameSelectVisibility;
            }
            set
            {
                _GameSelectVisibility = value;
                OnPropertyChanged("GameSelectVisibility");
            }
        }
        #endregion

        #region Data

        #region ModLibrary

        private List<LibraryMod> _Mods;
        public List<LibraryMod> Mods
        {
            get
            {
                return _Mods;
            }
            set
            {
                _Mods = value;
            }
        }

        private ObservableCollection<ModListItem> _ListMods { get; set; }
        public ObservableCollection<ModListItem> ListMods
        {
            get
            {
                return _ListMods;
            }
            set
            {
                _ListMods = value;

                OnPropertyChanged("ListMods");
            }
        }

        public ObservableCollection<LibraryMod> WorkingModList;
        #endregion

        #region API Library

        private List<Game> _Games { get; set; }
        public List<Game> Games
        {
            get
            {
                return _Games;
            }
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        private Game _CurrentGame { get; set; }
        public Game CurrentGame
        {
            get
            {
                return _CurrentGame;
            }
            set
            {
                _CurrentGame = value;
                setGameIMT();
                setGameDataCategories();
                GameSelectOverlayDisplay = value.ID == -1 ? true : false;
                OnPropertyChanged("CurrentGame");
            }
        }

        private GameModType _CurrentGameApiModType;
        public GameModType CurrentGameApiModType
        {
            get
            {
                return _CurrentGameApiModType;
            }
            set
            {
                _CurrentGameApiModType = value;
                setGameAPICategories();
                OnPropertyChanged("CurrentGameApiModType");
            }
        }

        private ObservableCollection<Category> _GameAPISubCategories;
        public ObservableCollection<Category> GameAPISubCategories
        {
            get
            {
                return _GameAPISubCategories;
            }
            set
            {
                _GameAPISubCategories = value;
                OnPropertyChanged("GameAPISubCategories");
            }
        }
        private void setGameAPICategories()
        {
            GameAPISubCategories = new ObservableCollection<Category>();
            if (CurrentGameApiModType != null)
            {
                List<Category> categories = CurrentGame.GameModTypes.Find(gmt => gmt.ID == CurrentGameApiModType.ID).Categories;
                foreach (Category c in categories)
                {
                    GameAPISubCategories.Add(c);
                }
            }

        }
        #endregion

        #region Content Library

        private List<ContentMapping> _ContentMappings;
        public List<ContentMapping> ContentMappings
        {
            get
            {
                return _ContentMappings;
            }
            set
            {
                _ContentMappings = value;
                OnPropertyChanged("ContentMappings");
            }
        }

        private ObservableCollection<ContentMapping> _AssociationContentMappings;
        public ObservableCollection<ContentMapping> AssociationContentMappings
        {
            get
            {
                return _AssociationContentMappings;
            }
            set
            {
                _AssociationContentMappings = value;
                OnPropertyChanged("AssociationContentMappings");
            }
        }

        private ObservableCollection<ContentMapping> _AssociationSlots;
        public ObservableCollection<ContentMapping> AssociationSlots
        {
            get
            {
                return _AssociationSlots;
            }
            set
            {
                _AssociationSlots = value;
                OnPropertyChanged("AssociationSlots");
            }
        }

        private ObservableCollection<ContentListItem> _ListContents { get; set; }
        public ObservableCollection<ContentListItem> ListContents
        {
            get
            {
                return _ListContents;
            }
            set
            {
                _ListContents = value;
                OnPropertyChanged("ListContents");
            }
        }

        #endregion

        #region Associations
        private List<Workspace> _QuasarWorkspaces;
        public List<Workspace> QuasarWorkspaces
        {
            get
            {
                return _QuasarWorkspaces;
            }
            set
            {
                _QuasarWorkspaces = value;
                OnPropertyChanged("QuasarWorkspaces");
            }
        }

        private Workspace _CurrentWorkspace;
        public Workspace CurrentWorkspace
        {
            get
            {
                return _CurrentWorkspace;
            }
            set
            {
                _CurrentWorkspace = value;
                OnPropertyChanged("CurrentWorkspace");
            }
        }
        #endregion


        #region Build
        private List<ModLoader> _ModLoaders;
        public List<ModLoader> ModLoaders
        {
            get => _ModLoaders;
            set
            {
                _ModLoaders = value;
                OnPropertyChanged("ModLoaders");
            }
        }

        private ObservableCollection<DriveInfo> _USBDrives;
        public ObservableCollection<DriveInfo> USBDrives
        {
            get
            {
                return _USBDrives;
            }
            set
            {
                _USBDrives = value;
                OnPropertyChanged("USBDrives");
            }
        }

        private ObservableCollection<string> _USBDriveLabels;
        public ObservableCollection<string> USBDriveLabels
        {
            get
            {
                return _USBDriveLabels;
            }
            set
            {
                _USBDriveLabels = value;
                OnPropertyChanged("USBDriveLabels");
            }
        }
        #endregion

        #region Game Data

        List<GameData> GameData { get; set; }

        private ObservableCollection<GameDataCategory> _GameDataCategories;
        public ObservableCollection<GameDataCategory> GameDataCategories
        {
            get
            {
                return _GameDataCategories;
            }
            set
            {
                _GameDataCategories = value;
                OnPropertyChanged("GameDataCategories");
            }
        }
        private void setGameDataCategories()
        {
            GameDataCategories = new ObservableCollection<GameDataCategory>();
            if (CurrentGame.ID != -1)
            {
                List<GameDataCategory> gdc = GameData.Find(g => g.GameID == CurrentGame.ID).Categories;
                foreach (GameDataCategory gd in gdc)
                {
                    GameDataCategories.Add(gd);
                }
            }
        }

        private GameDataCategory _IMTCurrentGDC;
        public GameDataCategory IMTCurrentGDC
        {
            get
            {
                return _IMTCurrentGDC;
            }
            set
            {
                _IMTCurrentGDC = value;
                OnPropertyChanged("IMTCurrentGDC");
            }
        }

        private GameDataCategory _AssociationCurrentGDC;
        public GameDataCategory AssociationCurrentGDC
        {
            get
            {
                return _AssociationCurrentGDC;
            }
            set
            {
                _AssociationCurrentGDC = value;
                OnPropertyChanged("AssociationCurrentGDC");
            }
        }
        #endregion

        #region Internal Mod Types
        List<InternalModType> InternalModTypes { get; set; }

        private ObservableCollection<InternalModType> _GameIMT;
        public ObservableCollection<InternalModType> GameIMT
        {
            get
            {
                return _GameIMT;
            }
            set
            {
                _GameIMT = value;
                OnPropertyChanged("GameIMT");
            }
        }
        private void setGameIMT()
        {
            GameIMT = new ObservableCollection<InternalModType>();
            if (CurrentGame.ID != -1)
            {
                List<InternalModType> internalModTypes = InternalModTypes.FindAll(imt => imt.GameID == CurrentGame.ID);
                foreach (InternalModType imt in internalModTypes)
                {
                    GameIMT.Add(imt);
                }
            }
        }

        private InternalModType _ContentCurrentIMT;
        public InternalModType ContentCurrentIMT
        {
            get
            {
                return _ContentCurrentIMT;
            }
            set
            {
                _ContentCurrentIMT = value;
                OnPropertyChanged("CurrentIMT");
            }
        }

        private ObservableCollection<InternalModType> _AssociationCorrespondingIMT;

        public ObservableCollection<InternalModType> AssociationCorrespondingIMT
        {
            get
            {
                return _AssociationCorrespondingIMT;
            }
            set
            {
                _AssociationCorrespondingIMT = value;
                OnPropertyChanged("AssociationCorrespondingIMT");
            }
        }
        #endregion

        #endregion

        #region Filters
        private void ShowOnlyNonFilteredMods(object sender, FilterEventArgs e)
        {/*
            ModListItem mle = e.Item as ModListItem;
            if (mle != null)
            {
                if (mle.Filter == false)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }*/
        }

        private void ShowOnlyNonFilteredContents(object sender, FilterEventArgs e)
        {
            ContentListItem cli = e.Item as ContentListItem;
            if (cli != null)
            {
                if (cli.Filter == false)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void ShowOnlyRelatedAssociation(object sender, FilterEventArgs e)
        {
            ContentListItem cli = e.Item as ContentListItem;
            if (cli != null)
            {
                if (cli.Filter == false)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }
        #endregion

        #region Sorting

        #endregion

        public bool readytoSelect { get; set; }

        MainUIViewModel MUVM { get; set; }
        public MainWindow()
        {
            

            //Pre-run checks
            bool Update = Checker.CheckQuasarUpdated();
            Folderino.CheckBaseFolders();
            Folderino.CompareReferences();
            bool Debug = false;
            Folderino.UpdateBaseFiles();

            int version = int.Parse(Properties.Settings.Default.AppVersion);
            int previous = int.Parse(Properties.Settings.Default.PreviousVersion);
            if (Update && version >= 1140 && previous < 1140)
            {
                String AssociationsPath = Properties.Settings.Default.DefaultDir + @"\Library\Associations.xml";
                String ContentPath = Properties.Settings.Default.DefaultDir + @"\Library\ContentMapping.xml";

                if (File.Exists(AssociationsPath))
                {
                    File.Delete(AssociationsPath);
                }
                if (File.Exists(ContentPath))
                {
                    File.Delete(ContentPath);
                }

            }

            Checker.BaseWorkspace();

            //Setting language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            MUVM = new MainUIViewModel();

            //Aww, here we go again
            InitializeComponent();

            try
            {
                CollectionViewSource cvs = new CollectionViewSource();
                cvs.Source = ListMods;
                cvs.Filter += ShowOnlyNonFilteredMods;



                if (Update && version >= 1140 && previous < 1140)
                {
                    ScanEverythingIntoWorkspace();
                }

                readytoSelect = true;

            }catch(Exception e)
            {
                MessageBoxResult result = MessageBox.Show("Quasar did not boot properly and crashed.", "Crash", MessageBoxButton.OK);
                Environment.Exit(0);
            }

            
            NewTabControl.DataContext = MUVM;
        }



        #region GameSelectOverlay
        private void ContentIMTSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GamesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readytoSelect)
            {
                readytoSelect = false;
                // Create a DoubleAnimation to animate the width of the button.
                DoubleAnimation myDoubleAnimation = new DoubleAnimation() { From = 1, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)) };

                // Configure the animation to target the button's Width property.
                Storyboard.SetTarget(myDoubleAnimation, OverlayGrid);
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("RenderTransform.ScaleX"));

                // Create a storyboard to contain the animation.
                Storyboard ReturnAnimation = new Storyboard();
                ReturnAnimation.Children.Add(myDoubleAnimation);

                Game selectedGame = (Game)GamesListView.SelectedItem;

                SelectGame(selectedGame);

                Properties.Settings.Default.LastSelectedGame = selectedGame.ID;
                Properties.Settings.Default.Save();

                ReturnAnimation.Begin();
            }

        }

        private void SelectGame(Game gamu)
        {
            CurrentGame = gamu;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentGame.ID != -1)
            {
                GamesListView.SelectedItem = GamesListView.Items[GamesListView.Items.IndexOf(CurrentGame)];
            }
            readytoSelect = true;
        }
        #endregion

        #region ModManagements

        //--------------------------------------
        //Mod Actions



        //Version actions
        private async void CheckUpdates(object sender, RoutedEventArgs e)
        {
            /*
            ModListItem element = (ModListItem)ManagementModListView.SelectedItem;
            if (element != null)
            {
                //Getting local Mod
                LibraryMod mod = Mods.Find(mm => mm.ID == element.LocalMod.ID && mm.TypeID == element.LocalMod.TypeID);
                Game game = Games.Find(g => g.ID == mod.GameID);
                GameModType mt = game.GameModTypes.Find(g => g.ID == mod.TypeID);
                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.GetAPIMod(mt.APIName, element.LocalMod.ID.ToString());

                //Create Mod from API information
                LibraryMod newmod = GetLibraryMod(newAPIMod, game);

                if (mod.Updates < newmod.Updates)
                {
                    string[] newDL = await APIRequest.GetDownloadFileName(mt.APIName, element.LocalMod.ID.ToString());
                    string quasarURL = APIRequest.GetQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.LocalMod.ID.ToString());
                    LaunchDownload(quasarURL);
                }
            }*/
        }

        public void Handler_TrashRequested(object sender, EventArgs e)
        {
            Boolean proceed = false;
            if (!Properties.Settings.Default.SupressModDeletion)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this mod ?", "Mod Deletion", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            

            if (proceed || Properties.Settings.Default.SupressModDeletion)
            {
                ModListItem item = (ModListItem)sender;

                //Removing from ContentMappings
                /*List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.ModID == item.LocalMod.ID);
                foreach (ContentMapping cm in relatedMappings)
                {
                    List<Association> associations = CurrentWorkspace.Associations.FindAll(ass => ass.ContentMappingID == cm.ID);
                    if (associations != null)
                    {
                        foreach (Association ass in associations)
                        {
                            CurrentWorkspace.Associations.Remove(ass);
                        }
                    }
                    ContentMappings.Remove(cm);
                }

                //Refreshing Contents
                ListContents = LoadContentMappings();

                //Removing from Library
                //Mods.Remove(item.LocalMod);

                //Refreshing Mods
                ListMods = LoadLibraryMods();

                //Writing changes
                Library.WriteModListFile(Mods);
                ContentXML.WriteContentMappingListFile(ContentMappings);
                SaveWorkspaces();*/
            }
        }

        public void Handler_AddRequested(object sender, EventArgs e)
        {
            /*
            ModListItem item = (ModListItem)sender;

            //Removing from ContentMappings
            List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.ModID == item.LocalMod.ID);
            foreach (ContentMapping cm in relatedMappings)
            {
                if (cm.GameDataItemID != -1)
                {
                    Association associations = CurrentWorkspace.Associations.Find(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot);
                    if (associations != null)
                    {
                        CurrentWorkspace.Associations[CurrentWorkspace.Associations.IndexOf(associations)] = new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot };
                    }
                    else
                    {
                        CurrentWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot });
                    }
                }
            }
            WorkspaceXML.WriteWorkspaces(QuasarWorkspaces);*/
        }

        #endregion

        #region Mod Association
        private void AssociationGameDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            AssociationCorrespondingIMT = new ObservableCollection<InternalModType>();

            GameDataCategory gdc = (GameDataCategory)AssociationGameDataList.SelectedItem;
            if (gdc != null)
            {
                AssociationCurrentGDC = gdc;
                List<InternalModType> correspondingTypes = InternalModTypes.FindAll(imt => imt.Association == gdc.ID);
                foreach (InternalModType imt in correspondingTypes)
                {
                    AssociationCorrespondingIMT.Add(imt);
                }

            }*/
        }
        private void AssociationGameElementChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSlots();
        }
        private void AssociationIMTChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSlots();
        }

        private void FilterSlots()
        {
            /*
            AssociationContentMappings = new ObservableCollection<ContentMapping>();
            AssociationSlots = new ObservableCollection<ContentMapping>();
            GameDataItem SelectedCategory = (GameDataItem)AssociationGameElementDataList.SelectedItem;
            InternalModType SelectedIMT = (InternalModType)AssociationTypeDataList.SelectedItem;

            if (SelectedIMT != null)
            {
                int AnyCategory = SelectedCategory == null ? -1 : SelectedCategory.ID;

                List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.GameDataItemID == AnyCategory && cm.InternalModType == SelectedIMT.ID);
                for (int i = 0; i < SelectedIMT.Slots; i++)
                {
                    AssociationSlots.Add(new ContentMapping() { Name = "Empty Slot n°" + (i + 1), SlotName = "FakeIMT" + (i + 1) });
                    
                }
                foreach (ContentMapping cm in relatedMappings)
                {
                    AssociationContentMappings.Add(cm);
                    List<Association> Slots = CurrentWorkspace.Associations.FindAll(asso => asso.ContentMappingID == cm.ID);
                    if(Slots != null)
                    {
                        foreach(Association ass in Slots)
                        {
                            setSlot(cm, ass.Slot);
                        }
                    }
                }
            }*/
        }

        private void setSlot(int indexSource, int indexDestination)
        {
            /*
            if(AssociationSlots.Count > 0)
            {
                ContentMapping DestinationMapping = AssociationContentMappings.ElementAt(indexSource);
                DestinationMapping.Slot = indexDestination;
                ContentMapping SourceMapping = (ContentMapping)ItemSlotListBox.Items.GetItemAt(indexDestination);
                Regex SlotRegex = new Regex(@"FakeIMT(\d[1-3])");
                if (!SlotRegex.IsMatch(SourceMapping.SlotName))
                    {
                        List<Association> asso = CurrentWorkspace.Associations.FindAll(a => a.ContentMappingID == SourceMapping.ID && a.Slot == SourceMapping.Slot);
                        foreach (Association a in asso)
                        {
                            CurrentWorkspace.Associations.Remove(a);
                        }
                    }
                

                AssociationSlots.RemoveAt(indexDestination);
                AssociationSlots.Insert(indexDestination, DestinationMapping);

                saveSlots();
            }*/
        }

        private void setSlot(ContentMapping input, int indexDestination)
        {
            if (AssociationSlots.Count > 0)
            {
                if(indexDestination < AssociationSlots.Count)
                {
                    AssociationSlots.RemoveAt(indexDestination);
                    input.Slot = indexDestination;
                    AssociationSlots.Insert(indexDestination, input);
                }
                
            }
            
        }

        private void saveSlots()
        {
            foreach(ContentMapping cm in AssociationSlots)
            {
                Regex SlotRegex = new Regex(@"FakeIMT(\d[1-3])");
                if (!SlotRegex.IsMatch(cm.SlotName))
                {
                    Association aa = CurrentWorkspace.Associations.Find(a => a.ContentMappingID == cm.ID && a.Slot == cm.Slot);
                    if (aa == null)
                    {
                        Association asso = new Association() { ContentMappingID = cm.ID, Slot = cm.Slot, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType };
                        CurrentWorkspace.Associations.Add(asso);
                    }
                }
            }

            SaveWorkspaces();
        }

        private void ResetSlotsButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            GameDataItem SelectedCategory = (GameDataItem)AssociationGameElementDataList.SelectedItem;
            InternalModType SelectedIMT = (InternalModType)AssociationTypeDataList.SelectedItem;

            if (SelectedIMT != null)
            {
                int AnyCategory = SelectedCategory == null ? -1 : SelectedCategory.ID;
                List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.GameDataItemID == AnyCategory && cm.InternalModType == SelectedIMT.ID);
                for (int i = 0; i < SelectedIMT.Slots; i++)
                {
                    AssociationSlots.Add(new ContentMapping() { Name = "Empty Slot n°" + (i + 1), SlotName = "FakeIMT" + (i + 1) });

                }
                foreach (ContentMapping cm in relatedMappings)
                {
                    AssociationContentMappings.Add(cm);
                    List<Association> Slots = CurrentWorkspace.Associations.FindAll(asso => asso.ContentMappingID == cm.ID);
                    if (Slots != null)
                    {
                        foreach (Association ass in Slots)
                        {
                            CurrentWorkspace.Associations.Remove(ass);
                        }
                    }
                }
            }*/


            SaveWorkspaces();
            FilterSlots();
        }

        #region Drag Drop
        private void ItemSourceListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void ItemSourceListBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ItemSourceListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = _dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var lb = sender as ListBox;
                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ItemSlotListBox_Drop(object sender, DragEventArgs e)
        {
            /*
            if (sender is Grid)
            {
                var source = e.Data.GetData(typeof(ContentMapping)) as ContentMapping;
                var target = (Grid)sender;
                Label l = (Label)target.Children[0];
                ContentMapping cm = AssociationSlots.Where(c => c.SlotName == l.Content.ToString()).ElementAt(0);

                int sourceIndex = ItemSourceListBox.Items.IndexOf(source);
                int targetIndex = ItemSlotListBox.Items.IndexOf(cm);

                setSlot(sourceIndex, targetIndex);
            }*/
        }

        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindVisualParent<T>(parentObject);
        }
        #endregion


        #endregion

        #region InternalModTypes
        //State changes
        private void InternalModTypeSelected(object sender, SelectionChangedEventArgs e)
        {/*
            //Getting info
            GameData gameData = GameData.Find(g => g.GameID == CurrentGame.ID);
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            IMTDataGrid.ItemsSource = type.Files;
            GameDataCategory cat = gameData.Categories.Find(c => c.ID == type.Association);

            //Resetting info
            IMTFileText.Text = "";
            IMTPathText.Text = "";
            IMTMandatory.IsChecked = false;
            IMTMandatory.IsEnabled = false;
            IMTFileText.IsEnabled = false;
            IMTPathText.IsEnabled = false;
            IMTSlotsText.IsEnabled = true;
            IMTAssotiationSelect.IsEnabled = true;

            //Displaying info
            IMTAssotiationSelect.SelectedItem = IMTAssotiationSelect.Items[IMTAssotiationSelect.Items.IndexOf(cat)];
            IMTSlotsText.Text = type.Slots.ToString();*/
        }

        private void DataGridRowSelected(object sender, SelectionChangedEventArgs e)
        {
            /*
            DataGrid selectedGrid = (DataGrid)sender;
            InternalModTypeFile file = (InternalModTypeFile)selectedGrid.SelectedItem;
            if (file != null)
            {
                IMTPathText.Text = file.SourcePath;
                IMTFileText.Text = file.SourceFile;
                //IMTDestinationText.Text = file.Destination;
                IMTMandatory.IsChecked = file.Mandatory;
                IMTFileText.IsEnabled = true;
                IMTPathText.IsEnabled = true;
                IMTMandatory.IsEnabled = true;
            }*/
        }
        private void IMTAssotiationSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            GameDataCategory category = (GameDataCategory)IMTAssotiationSelect.SelectedItem;
            if (category.ID == 0)
            {
                IMTSlotsText.IsEnabled = false;
                IMTCustomNameText.IsEnabled = true;
                IMTSlotsText.Text = "1";
            }
            else
            {
                IMTSlotsText.IsEnabled = true;
                IMTCustomNameText.IsEnabled = false;
                IMTCustomNameText.Text = "";
                InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
                IMTSlotsText.Text = type.Slots.ToString();

            }*/
        }

        private void IMTModLoaderSelected(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (IMTModLoaderCombo.SelectedIndex == -1)
            {
                IMTBuilderOutputFilePath.IsEnabled = false;
                IMTBuilderOutputFilePath.Text = "";

                IMTBuilderOutputFolderPath.IsEnabled = false;
                IMTBuilderOutputFolderPath.Text = "";
            }
            else
            {
                IMTBuilderOutputFilePath.IsEnabled = true;
                IMTBuilderOutputFolderPath.IsEnabled = true;
                ModLoader GB = (ModLoader)IMTModLoaderCombo.SelectedItem;
                InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
                if( file != null)
                {
                    BuilderFolder BFol = file.Destinations.Find(f => f.BuilderID == GB.ID);
                    BuilderFile BFil = file.Files.Find(f => f.BuilderID == GB.ID);

                    IMTBuilderOutputFilePath.Text = BFil.Path;
                    IMTBuilderOutputFolderPath.Text = BFol.Path;
                }
            }*/

        }

        //IMT Actions
        private void IMTAddFile(object sender, RoutedEventArgs e)
        {
            /*
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            if (type != null)
            {
                type.Files.Add(new InternalModTypeFile());
                IMTDataGrid.Items.Refresh();
            }*/
        }

        private void IMTDeleteFile(object sender, RoutedEventArgs e)
        {
            /*
            InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
            if (file != null)
            {
                InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
                if (type != null)
                {
                    type.Files.Remove(file);
                    IMTDataGrid.Items.Refresh();
                }
            }*/
        }

        //Saves
        private void IMTInfoSave(object sender, RoutedEventArgs e)
        {/*
            InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
            if (file != null)
            {
                file.SourcePath = IMTPathText.Text;
                file.SourceFile = IMTFileText.Text;
                //file.Destination = IMTDestinationText.Text;
                file.Mandatory = IMTMandatory.IsChecked ?? false;
            }
            IMTDataGrid.Items.Refresh();*/
        }
        private void IMTSaveXML(object sender, RoutedEventArgs e)
        {/*
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            if (type != null)
            {
                int i = 0;
                foreach (InternalModTypeFile file in type.Files)
                {
                    file.ID = i;
                    i++;
                }
                GameDataCategory cat = (GameDataCategory)IMTAssotiationSelect.SelectedItem;
                type.Association = cat.ID;
                type.Slots = Int32.Parse(IMTSlotsText.Text);
                XML.SaveInternalModType(type);
            }
            */

        }

        private void IMTTestFile(object sender, RoutedEventArgs e)
        {
            /*
            if(ManagementModListView.SelectedIndex != -1){
                ModListItem lm = (ModListItem)ManagementModListView.SelectedItem;
                ModFileManager mfm = new ModFileManager(lm.LocalMod, CurrentGame);
                new DefinitionsWindow(mfm, IMTFileText.Text, IMTBuilderOutputFilePath.Text, IMTPathText.Text, IMTBuilderOutputFolderPath.Text, GameDataCategories.ToList(), GameIMT.ToList(), (int)IMTModLoaderCombo.SelectedIndex).Show();
            }*/
        }

        #endregion

        #region Build
        private async void Build_Button(object sender, RoutedEventArgs e)
        {
            /*
            bool willrun = true;
            string address = BuildFTPAddress.Text;
            string port = BuildFTPPort.Text;

            //Checking ModLoader
            if (BuilderModLoaderCombo.SelectedIndex == -1)
            {
                BuilderLogs.Text += "Please select a modloader first\r\n";
                willrun = false;
            }

            //Checking FTP
            if (BuilderFTPRadio.IsChecked == true)
            {
                if (!validateIP() || !validatePort())
                {
                    willrun = false;
                }
            }

            //Checking Local Transfer
            if (BuilderSDCombo.SelectedIndex == -1 && BuilderLocalRadio.IsChecked == true)
            {
                BuilderLogs.Text += "Please select a SD Drive first\r\n";
                willrun = false;
            }

           


            if (willrun)
            {
                Properties.Settings.Default.ModLoader = BuilderModLoaderCombo.SelectedIndex;
                Properties.Settings.Default.Wireless = (bool)BuilderFTPRadio.IsChecked;
                Properties.Settings.Default.Save();

                BuilderBuild.IsEnabled = false;
                BuilderFTPTest.IsEnabled = false;
                Boolean proceed = false;
                if (!Properties.Settings.Default.SupressBuildDeletion)
                {
                    MessageBoxResult result = MessageBox.Show("You are about to build the workspace. This will wipe your Workspace on your Switch to avoid conflicts. Do you wish to proceed with the build process?", "File Deletion Warning", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            proceed = true;
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                if (proceed || Properties.Settings.Default.SupressBuildDeletion)
                {
                    BuilderProgress.IsIndeterminate = true;
                    QuasarTaskBar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;

                    string pathname = BuilderSDCombo.SelectedIndex == -1 ? "" : USBDrives[BuilderSDCombo.SelectedIndex].Name;
                    string ftpPath = address + ":" + port;
                    NetworkCredential NC = null;
                    if (BuildPWRadio.IsChecked == true)
                    {
                        NC = new NetworkCredential(BuildFTPUN.Text, BuildFTPPW.Text);
                        
                    }
                    if (BuilderLocalRadio.IsChecked == true)
                    {
                        ftpPath = "";
                    }


                    ModLoader gamubuilder = (ModLoader)BuilderModLoaderCombo.SelectedItem;
                    await Builder.SmashBuild(pathname, gamubuilder.ID, ftpPath, NC, BuilderWipeCreateRadio.IsChecked == true ? 1 : -1, Mods, ContentMappings, CurrentWorkspace, InternalModTypes, CurrentGame, GameData, BuilderLogs, BuilderProgress,ModLoaders.ElementAt(BuilderModLoaderCombo.SelectedIndex), QuasarTaskBar);
                    BuilderProgress.Value = 100;
                    QuasarTaskBar.ProgressValue = 100;
                    BuilderLogs.Text += "Done\r\n";
                    BuilderBuild.IsEnabled = true;
                    BuilderFTPTest.IsEnabled = true;

                    CurrentWorkspace.Built = true;
                    WorkspaceXML.WriteWorkspaces(QuasarWorkspaces);
                }

            }*/
        }

        #endregion

        #region Settings
        private void SaveWorkspaces()
        {
            Workspace item = QuasarWorkspaces.Find(w => w.ID == CurrentWorkspace.ID);
            QuasarWorkspaces[QuasarWorkspaces.IndexOf(item)] = CurrentWorkspace;
            WorkspaceXML.WriteWorkspaces(QuasarWorkspaces);
        }

        #endregion

        #region Workspaces

        public void SetCurrentWorkspace(Workspace workspace)
        {
            CurrentWorkspace = workspace;
        }

        public void setTouchmARCWorkspace(object sender, RoutedEventArgs e)
        {
            if (CurrentWorkspace.Built)
            {
                if (Properties.Settings.Default.FTPValid)
                {
                    try
                    {
                        FtpClient ftp = new FtpClient(Properties.Settings.Default.FTPIP);
                        ftp.Port = int.Parse(Properties.Settings.Default.FTPPort);
                        if (Properties.Settings.Default.FTPUN != "")
                        {
                            ftp.Credentials = new NetworkCredential(Properties.Settings.Default.FTPUN, Properties.Settings.Default.FTPPW);
                        }

                        bool distant = TouchmARC.GetDistantConfig(ftp);
                        if (distant)
                        {
                            TouchmARC.ModifyTouchmARCConfig(CurrentWorkspace.Name + "/arc", CurrentWorkspace.Name + "/stream");
                            TouchmARC.SendDistantConfig(ftp);
                        }
                        MessageBoxResult result = MessageBox.Show("Workspace activated", "Success", MessageBoxButton.OK);
                    }
                    catch(Exception ez)
                    {
                        MessageBoxResult result = MessageBox.Show("Could not send the new config file. \r\n "+ez.Message, "Error", MessageBoxButton.OK);
                    }
                    
                }
            }
            
        }
        #endregion

        #region Detection

        private bool FirstScanLibraryMod(LibraryMod libraryMod, Game game, List<InternalModType> types)
        {
            bool processed = false;
            List<ContentMapping> SearchList = Searchie.AutoDetectinator(libraryMod, types, game, GameData);

            List<ContentMapping> WorkingList = ContentMappings;
            foreach (ContentMapping cm in SearchList)
            {
                WorkingList.Add(cm);
                processed = true;
            }
            ContentMappings = WorkingList;
            AutoSlotDetectedItems(SearchList);
            ContentXML.WriteContentMappingListFile(ContentMappings);

            return processed;
        }

        private void DetectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            DetectionTreeView.Items.Clear();
            ContentMapping m = (ContentMapping)DetectionList.SelectedItem;
            if(DetectionList.SelectedIndex != -1)
            {
                foreach (ContentMappingFile cmf in m.Files)
                {
                    DetectionTreeView.Items.Add(new TreeViewItem() { Header = cmf.SourcePath});
                }
            }
            */
        }

        private void ScanEverythingIntoWorkspace()
        {
            foreach(LibraryMod lm in Mods)
            {
                FirstScanLibraryMod(lm, CurrentGame, InternalModTypes);
            }
        }
        private void AutoSlotDetectedItems(List<ContentMapping> elements)
        {
            foreach(ContentMapping cm in elements)
            {
                if(cm.GameDataItemID != -1)
                {
                    Association associations = CurrentWorkspace.Associations.Find(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot);
                    if (associations != null)
                    {
                        CurrentWorkspace.Associations[CurrentWorkspace.Associations.IndexOf(associations)] = new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot };
                    }
                    else
                    {
                        CurrentWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot });
                    }
                }
            }
            SaveWorkspaces();
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
            try
            {
                QuasarTaskBar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                bool newElement = false;
                string downloadText = "";
                ModListItem mli = new ModListItem(true);

                //Setting base ModFileManager
                ModFileManager ModFileManager = new ModFileManager(_URL);

                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.GetAPIMod(ModFileManager.APIType, ModFileManager.ModID);

                //Finding related game
                Game game = Games.Find(g => g.GameName == newAPIMod.GameName);

                //Resetting ModFileManager based on new info
                ModFileManager = new ModFileManager(_URL, game);

                //Setting game UI
                //mli.setGame(game);

                //Finding existing mod
                LibraryMod Mod = Mods.Find(mm => mm.ID == Int32.Parse(ModFileManager.ModID) && mm.TypeID == Int32.Parse(ModFileManager.ModTypeID));

                //Create Mod from API information
                LibraryMod newmod = GetLibraryMod(newAPIMod, game);

                bool needupdate = true;
                //Checking if Mod is already in library
                if (Mod != null)
                {
                    if (Mod.Updates < newmod.Updates)
                    {/*
                        var query = ListMods.Where(ml => ml.LocalMod == Mod);
                        mli = query.ElementAt(0);
                        downloadText = "Updating mod";*/
                    }
                    else
                    {
                        needupdate = false;
                    }
                }
                else
                {
                    Mod = new LibraryMod(Int32.Parse(ModFileManager.ModID), Int32.Parse(ModFileManager.ModTypeID), false);
                    newElement = true;
                    downloadText = "Downloading new mod";
                }
                if (!WorkingModList.Contains(Mod) && needupdate)
                {
                    WorkingModList.Add(Mod);

                    //Setting up new ModList
                    if (newElement)
                    {
                        //Adding element to list
                        Mods.Add(newmod);
                        ListMods.Add(mli);
                    }
                    else
                    {
                        //Updating List
                        Mods[Mods.IndexOf(Mod)] = newmod;
                    }

                    //Setting download UI
                    //mli.ModStatusValue = downloadText;

                    Downloader modDownloader = new Downloader(mli);

                    //Wait for download completion
                    await modDownloader.DownloadArchiveAsync(ModFileManager);

                    //Setting extract UI
                    //mli.ModStatusValue = "Extracting mod";

                    //Preparing Extraction
                    Unarchiver un = new Unarchiver(mli);

                    //Wait for Archive extraction
                    await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, ModFileManager.ModArchiveFormat);

                    //Setting extract UI
                    //mli.ModStatusValue = "Moving files";

                    //Moving files
                    await ModFileManager.MoveDownload();

                    //Cleanup
                    ModFileManager.ClearDownloadContents();

                    //Getting Screenshot from Gamebanana
                    await APIRequest.GetScreenshot(ModFileManager.APIType, ModFileManager.ModID, game.ID.ToString(), Mod.TypeID.ToString(), Mod.ID.ToString());


                    //Providing mod to ModListElement and showing info
                    //mli.SetMod(newmod);
                    //mli.Downloaded = true;

                    CollectionViewSource cvs = (CollectionViewSource)this.Resources["CollectionOMods"];
                    cvs.View.Refresh();

                    //Scanning Files
                    int modIndex = Mods.IndexOf(Mod);
                    if (modIndex == -1)
                    {
                        Mods[Mods.IndexOf(newmod)].FinishedProcessing = FirstScanLibraryMod(newmod, game, InternalModTypes);
                    }
                    else
                    {
                        Mods[modIndex].FinishedProcessing = FirstScanLibraryMod(newmod, game, InternalModTypes);
                    }

                    //Refreshing  Interface
                    //mli.Operation = false;


                    //Saving XML
                    WriteModListFile(Mods);

                    //Removing mod from Working List
                    WorkingModList.Remove(Mod);
                    QuasarTaskBar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                }
            }
            catch(Exception e)
            {
                MessageBoxResult result = MessageBox.Show("One of Quasar's downloads failed. The app will shutdown due to a potential unknown state. Sorry !", "Crash", MessageBoxButton.OK);
                Environment.Exit(0);
            }
            
        }


        #endregion

        private void ItemSlotListBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            ContentMapping cm = (ContentMapping)ItemSlotListBox.SelectedItem;
            int index = AssociationSlots.IndexOf(cm);
            Regex SlotRegex = new Regex(@"FakeIMT(\d[1-3])");

            if (!SlotRegex.IsMatch(AssociationSlots.ElementAt(index).Name))
            {
                AssociationSlots.RemoveAt(index);
                AssociationSlots.Insert(index,new ContentMapping() { Name = "Empty Slot n°" + (index + 1), SlotName = "FakeIMT" + (index + 1) });
            }

            SaveWorkspaces();
            FilterSlots();*/
        }
    }
    
}
