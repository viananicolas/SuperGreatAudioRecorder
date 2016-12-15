using System;
using System.Globalization;
using Android.App;
using Android.Media;
using Android.Media.Audiofx;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Java.IO;

namespace SuperGreatAudioRecorder
{
    [Activity(Label = "SuperGreatAudioRecorder", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        MediaRecorder _recorder;
        MediaPlayer _player;
        private NoiseSuppressor _noiseSuppressor;
        Button _start;
        Button _stop;
        string path = "/storage/emulated/0/"+DateTime.Now+".3gpp";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            File folder = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments) +
                             File.Separator + "SGAR"+File.Separator+DateTime.Now.ToString("yyyyMMddHHmmss") + ".3gpp");
            if(!folder.Exists())
                folder.Mkdir();
            //folder.Mkdir();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            _start = FindViewById<Button>(Resource.Id.start);
            _stop = FindViewById<Button>(Resource.Id.stop);
            _start.Click += delegate {
                _stop.Enabled = !_stop.Enabled;
                _start.Enabled = !_start.Enabled;
                //_recorder = new AudioRecord(AudioSource.Mic, 4800, ChannelIn.Stereo, Encoding.Pcm16bit, 16);
                _recorder.SetAudioSource(AudioSource.Mic);
                _recorder.SetOutputFormat(OutputFormat.AacAdts);
                _recorder.SetAudioEncoder(AudioEncoder.AacEld);
                _recorder.SetAudioSamplingRate(48000);
                _recorder.SetAudioChannels(2);
                _recorder.SetOutputFile(folder.Path);
                _recorder.Prepare();
                _recorder.Start();
                //_noiseSuppressor = NoiseSuppressor.Create(_recorder.AudioSessionId);
                //_recorder.Start();
            };
            _stop.Click += delegate {
                _stop.Enabled = !_stop.Enabled;

                _recorder.Stop();
                _recorder.Release();

                _player.SetDataSource(folder.Path);
                _player.Prepare();
                _player.Start();
            };
        }
        protected override void OnResume()
        {
            base.OnResume();

            _recorder = new MediaRecorder();
            _player = new MediaPlayer();

            _player.Completion += (sender, e) => {
                _player.Reset();
                _start.Enabled = !_start.Enabled;
            };

        }
        protected override void OnPause()
        {
            base.OnPause();

            _player.Release();
            _recorder.Release();
            _player.Dispose();
            _recorder.Dispose();
            _player = null;
            _recorder = null;
        }
    }
}

