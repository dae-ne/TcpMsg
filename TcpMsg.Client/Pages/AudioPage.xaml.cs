﻿using System.Media;
using System.Windows.Controls;
using TcpMsg.Client.Media;

namespace TcpMsg.Client.Pages
{
    public partial class AudioPage : Page
    {
        private readonly Media.Audio _audio;
        private SoundPlayer _player;

        public AudioPage(Media.Audio audio)
        {
            InitializeComponent();
            _audio = audio;
        }

        private void PlayBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using var ms = _audio.GetMemoryStream();
            _player = new SoundPlayer(ms);
            _player.Play();
        }

        private void StopBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _player.Stop();
        }
    }
}
