﻿using BlueMuse.Helpers;
using BlueMuse.MuseManagement;
using System.Linq;
using System.Windows.Input;
using BlueMuse.Bluetooth;
using System.Threading;

namespace BlueMuse.ViewModels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class MainPageVM : ObservableObject
    {
        private BluetoothManager museManager;
        public ObservableCollection<Muse> Muses { get; set; }
        private Muse selectedMuse; // Tracks user selection from list.
        public Muse SelectedMuse { get { return selectedMuse; } set { selectedMuse = value; if (value != null) SetSelectedMuse(value); } }
        private string searchText = string.Empty;
        public string SearchText { get { return searchText; } set { SetProperty(ref searchText, value); } } 

        public MainPageVM()
        {
            Muses = new ObservableCollection<Muse>();
            museManager = new BluetoothManager(Muses);
            museManager.FindMuses();
            new Timer(SearchTextAnimate, null, 0, 600);
        }

        private void SearchTextAnimate(object state)
        {
            string baseText = "Searching for Muses";
            if (searchText.Count(x => x == '.') == 0)
                SearchText = baseText + ".";
            else if (searchText.Count(x => x == '.') == 1)
                SearchText = baseText + "..";
            else if (searchText.Count(x => x == '.') == 2)
                SearchText = baseText + "...";
            else SearchText = baseText;
        }

        private ICommand forceRefresh;
        public ICommand ForceRefresh
        {
            get
            {
                return forceRefresh ?? (forceRefresh = new CommandHandler(() =>
                {
                    museManager.ForceRefresh();
                }, true));
            }
        }

        private ICommand startStreaming;
        public ICommand StartStreaming
        {
            get
            {
                return startStreaming ?? (startStreaming = new CommandHandler((param) =>
                {
                    museManager.StartStreaming(param);
                }, true));
            }
        }

        private ICommand stopStreaming;
        public ICommand StopStreaming
        {
            get
            {
                return stopStreaming ?? (stopStreaming = new CommandHandler((param) =>
                {
                    museManager.StopStreaming(param);
                }, true));
            }
        }

        private void SetSelectedMuse(Muse muse)
        {
            var selectedMuses = Muses.Where(x => x.IsSelected);
            foreach (var selectedMuse in selectedMuses)
            {
                selectedMuse.IsSelected = false;
            }
            muse.IsSelected = true;
        }
    }
}
