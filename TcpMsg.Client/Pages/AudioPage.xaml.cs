﻿using System.Media;
using System.Windows.Controls;
using TcpMsg.Client.Media;

namespace TcpMsg.Client.Pages
{
    public partial class AudioPage : Page
    {
        private readonly Audio _audio;
        private SoundPlayer _player;

        public AudioPage(Audio audio)
        {
            InitializeComponent();
            _player = new SoundPlayer();
            _audio = audio;
        }

        private void PlayBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using var ms = _audio.GetMemoryStream();
            _player.Stream = ms;
            _player.Play();
        }

        private void StopBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _player.Stop();
        }
    }
}
