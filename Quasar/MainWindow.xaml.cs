﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Quasar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool[] progressStates = new bool[] { false, false, false };
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(Quasar.Properties.Settings.Default["language"].ToString());

            InitializeComponent();

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Task<Member> getMember = APIRequest.RunAsync("1513589");
            
            PgrLabel1.Content = "Pulling Name";

            Member mow = await getMember;

            PgrLabel1.Content = mow.name;
        }
    }
}
