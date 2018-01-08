using Media.DataModel;
using Media.Player;
using Media.Controller.Ex01;
using Media.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Media.WPF.Ex01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaController _activeController;
        MusicController _musicController;
        MovieController _movieController;
        bool IsPaused = true; //cheatcode
        byte [] _newFile;

        public MainWindow()
        {
            InitializeComponent();

            //controller objecten aanmaken na de referentie gelegd te hebben
            _musicController = new MusicController(new AudioPlayer(), new AudioPlaylist());
            _movieController = new MovieController();

            //Starten en stoppen van de player
            _musicController.Player.IsStarted += SetMusicPlay;
            _musicController.Player.IsFinished += SetMusicPlay;

            //listboxen opvullen
            musicTabItem.DataContext = _musicController.List;
            movieTabItem.DataContext = _movieController.List;
            playlistListBox.DataContext = _musicController.PlayList.List;

            //Playlist opvullen
            AddToPlaylistButton.Click += AddToPlaylistButton_Click;

            //CleanUp
            mainWindow.Closed += MainWindow_Closed;

            //TabControl: bijhouden welke de ActiveController is
            tabControl.SelectionChanged += TabControl_SelectionChanged;

            //Andere Eventhandlers
            musicListBox.SelectionChanged += ListBox_SelectionChanged;
            movieListBox.SelectionChanged += ListBox_SelectionChanged;
            playlistListBox.SelectionChanged += PlaylistListBox_SelectionChanged;
            AddMusicFileButton.Click += AddFileButton_Click;
            AddMoviefileButton.Click += AddFileButton_Click;
            SaveMusicButton.Click += SaveMusicButton_Click;
            SaveMovieButton.Click += SaveMovieButton_Click;
            CancelMusicButton.Click += CancelButton_Click;
            CancelMovieButton.Click += CancelButton_Click;
            DeleteMusicButton.Click += DeleteButton_Click;
            DeleteMovieButton.Click += DeleteButton_Click;

            CloseMenuItem.Click += CloseMenuItem_Click;

            //Playlist buttons
            PlayMusicButton.Click += PlayMusicButton_Click;
            PauseMusicButton.Click += PauseMusicButton_Click;
            StopMusicButton.Click += StopMusicButton_Click;
            NextMusicButton.Click += NextMusicButton_Click;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

            //Play Movie
            PlayMovieButton.Click += PlayMovieButton_Click;
        }

        private void ClearSelection()
        {
            if (_activeController == _musicController)
            {
                DeleteMusicButton.IsEnabled = false;
                AddToPlaylistButton.IsEnabled = false;
                PlayMusicButton.IsEnabled = false;
                NextMusicButton.IsEnabled = false;
                StopMusicButton.IsEnabled = false;
                PauseMusicButton.IsEnabled = false;
                VolumeSlider.IsEnabled = false;
                musicListBox.SelectedItem = null;
                playlistListBox.SelectedItem = null;
                _musicController.ClearSelected();
                _newFile = null;
            }
            else if (_activeController == _movieController)
            {
                DeleteMovieButton.IsEnabled = false;
                PlayMovieButton.IsEnabled = false;
                movieListBox.SelectedItem = null;
                _movieController.ClearSelected();
                _newFile = null;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (musicTabItem.IsSelected)
            {
                _activeController = _musicController;
            }
            else if (movieTabItem.IsSelected)
            {
                _activeController = _movieController;
            }
            ClearSelection();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_activeController == _musicController && musicListBox.SelectedItem != null)
            {
                playlistListBox.SelectedItem = null;
                selectedSongDockPanel.DataContext = _musicController.List;
                var song = (Song)musicListBox.SelectedItem;
                if (song != null)
                {
                    _musicController.ChangeSelected(song);
                    SetMusicForm();
                    _newFile = null; // zorgt ervoor dat als song verandert, de added file niet gesavet wordt bij een andere song
                }
            }
            else if (_activeController == _movieController)
            {
                var movie = (Movie)movieListBox.SelectedItem;
                if (movie != null)
                {
                    _movieController.ChangeSelected(movie);
                    SetMovieForm();
                    _newFile = null; //idem
                }
            }
            e.Handled = true;
        }

        private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(playlistListBox.SelectedItem != null)
            {
                musicListBox.SelectedItem = null;
                selectedSongDockPanel.DataContext = _musicController.PlayList.List;
                var song = (Song)playlistListBox.SelectedItem;
                if (song != null)
                {
                    _musicController.ChangeSelected(song);
                    SetMusicForm();
                }
            }
            e.Handled = true;
        }

        private void SetMusicForm()
        {
            if (musicListBox.SelectedItem != null || playlistListBox.SelectedItem != null)
            {
                DeleteMusicButton.IsEnabled = true;
                if (_musicController.Selected.File != null)
                {
                    AddToPlaylistButton.IsEnabled = true;
                } 
                else
                {
                    AddToPlaylistButton.IsEnabled = false;
                }
            }
        }

        private void SetMovieForm()
        {
            if (movieListBox.SelectedItem != null)
            {
                DeleteMovieButton.IsEnabled = true;
                if (_movieController.Selected.File != null)
                {
                    PlayMovieButton.IsEnabled = true;
                }
                else
                {
                    PlayMovieButton.IsEnabled = false;
                }
            }
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = "Add file to media",
                Filter = _activeController.FileFilter()
            };
            dialog.ShowDialog();
            if (dialog.FileName != "")
            {
                var file = LoadConvert.ImportFile(dialog.FileName);
                _newFile = file;
                if (_activeController == _musicController)
                {
                    SetMusicForm();
                }
                else if (_activeController == _movieController)
                {
                    SetMovieForm();
                }
            }
        }
        
        private void SaveMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (SingerTextBox.Text != "" && MusicTitleTextBox.Text != "")
            {
                if (_musicController.Selected == null)
                {
                    var newSong = new Song()
                    {
                        Singer = SingerTextBox.Text,
                        Title = MusicTitleTextBox.Text,
                        File = _newFile
                    };
                    _musicController.AddMedia(newSong);
                }
                else
                {
                    var selectedSong = (Song)_musicController.Selected;
                    selectedSong.Singer = SingerTextBox.Text;
                    selectedSong.Title = MusicTitleTextBox.Text;
                    if (selectedSong.File == null)
                    {
                        selectedSong.File = _newFile;
                    } else if (selectedSong.File != _newFile && _newFile != null)
                    {
                        selectedSong.File = _newFile;
                    }
                    _musicController.ChangeSelected(_musicController.Selected);
                    musicListBox.Items.Refresh();
                    playlistListBox.Items.Refresh();
                }
                ClearSelection();
                SetMusicPlay();
            }
            else
            {
                MessageBox.Show("Please fill in all the fields!");
            }
        }

        private void SaveMovieButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectorTextBox.Text != "" && MovieTitleTextBox.Text != "")
            {
                if (_movieController.Selected == null)
                {
                    var newMovie = new Movie()
                    {
                        Director = DirectorTextBox.Text,
                        Title = MovieTitleTextBox.Text,
                        File = _newFile
                    };
                    _movieController.AddMedia(newMovie);
                }
                else
                {
                    var selectedMovie = (Movie)_movieController.Selected;
                    selectedMovie.Director = DirectorTextBox.Text;
                    selectedMovie.Title = MovieTitleTextBox.Text;
                    if (selectedMovie.File == null)
                    {
                        selectedMovie.File = _newFile;
                    }
                    else if (selectedMovie.File != _newFile && _newFile != null)
                    {
                        selectedMovie.File = _newFile;
                    }
                    _movieController.ChangeSelected(_movieController.Selected);
                    movieListBox.Items.Refresh();
                }
                ClearSelection();
            }
            else
            {
                MessageBox.Show("Please fill in all the fields!");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();

            // zorgen dat als er muziek aant afspelen is of muziek in de playlist zit, er rekening wordt gehouden met het clearen van die knoppen...
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeController == _musicController)
            {
                if (musicListBox.SelectedItem != null)
                {
                    var song = (Song)musicListBox.SelectedItem;
                    _musicController.List.Remove(song);
                    ClearSelection();
                }
            }
            else if (_activeController == _movieController)
            {
                if (movieListBox.SelectedItem != null)
                {
                    var movie = (Movie)movieListBox.SelectedItem;
                    _movieController.List.Remove(movie);
                    ClearSelection();
                }
            }
        }

        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            _musicController.AddSelectedToPlaylist();
            SetMusicPlay();
        }

        private void SetMusicPlay()
        {
            if (_musicController.HasSongsInPlaylist)
            {
                if (!_musicController.IsPlaying)
                {
                    PlayMusicButton.IsEnabled = true;
                    NextMusicButton.IsEnabled = false;
                    PauseMusicButton.IsEnabled = false;
                    StopMusicButton.IsEnabled = false;
                    VolumeSlider.IsEnabled = false;
                }
                else
                {
                    PlayMusicButton.IsEnabled = false;
                    NextMusicButton.IsEnabled = true;
                    PauseMusicButton.IsEnabled = true;
                    StopMusicButton.IsEnabled = true;
                    VolumeSlider.IsEnabled = true;
                }
            }
            else
            {
                if (!_musicController.IsPlaying)
                {
                    PlayMusicButton.IsEnabled = false;
                    NextMusicButton.IsEnabled = false;
                    PauseMusicButton.IsEnabled = false;
                    StopMusicButton.IsEnabled = false;
                    VolumeSlider.IsEnabled = false;
                }
                else
                {
                    PlayMusicButton.IsEnabled = false;
                    NextMusicButton.IsEnabled = false;
                    PauseMusicButton.IsEnabled = true;
                    StopMusicButton.IsEnabled = true;
                    VolumeSlider.IsEnabled = true;
                }
                
            }
        }

        private void PlayMusicButton_Click(object sender, RoutedEventArgs e)
        {
            var song = _musicController.PlayFromPlaylist();
            NowPlayingLabel.Content = "Now Playing: " + song.Singer + " - " + song.Title;
            SetMusicPlay();
        }

        private void PauseMusicButton_Click(object sender, RoutedEventArgs e)
        {
            _musicController.Pause();
            if (IsPaused)
            {
                PauseMusicButton.Content = "Resume";
                IsPaused = false;
            } else
            {
                PauseMusicButton.Content = "Pause";
                IsPaused = true;
            }

        }
        
        private void StopMusicButton_Click(object sender, RoutedEventArgs e)
        {
            _musicController.StopPlaying();
            NowPlayingLabel.Content = "Now Playing: ";
            SetMusicPlay();

            // zorgen dat als het muziek gepaused is, de Text van de Pauseknop terug op "Pause" komt te staan
        }

        private void NextMusicButton_Click(object sender, RoutedEventArgs e)
        {
            PlayMusicButton_Click(sender, e);
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _musicController.Volume = (float) VolumeSlider.Value;
        }

        private void PlayMovieButton_Click(object sender, RoutedEventArgs e)
        {
            new VideoPlayer(_activeController.Selected.File).ShowDialog();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _musicController.Dispose();
            _movieController.Dispose();
        }

    }
}
