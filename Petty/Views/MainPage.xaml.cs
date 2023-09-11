using Petty.Platforms.Android.Audio;
using Petty.Services.Local.Audio;
using Petty.Services.Local.PermissionsFolder;
using Petty.Services.Local.PettyCommands;
using System.Globalization;

namespace Petty.Views;

public partial class MainPage : ContentPage, IProgress<string>
{
    public MainPage(
        LoggerService logger,
        MainViewModel mainViewModel,
        PermissionService permissionService,
        PettyCommandsService pettyCommandsService)
    {
        BindingContext = mainViewModel;
        InitializeComponent();

        //recorder = new Recorder();
        recorder = new AudioRecorderService(pettyCommandsService, logger)
        {
            StopRecordingAfterTimeout = true,
            TotalAudioTimeout = TimeSpan.FromSeconds(15),
            AudioSilenceTimeout = TimeSpan.FromSeconds(2)
        };

        player = new AudioPlayer();
        player.FinishedPlaying += Player_FinishedPlaying;
        _permissionService = permissionService;
        _pettyCommandsService = pettyCommandsService;
    }

    AudioPlayer player;
    AudioRecorderService recorder;
    //Recorder recorder;

    async void Record_Clicked(object sender, EventArgs e)
    {
        await RecordAudio();
    }
    private CancellationTokenSource _token = new CancellationTokenSource();
    private readonly PermissionService _permissionService;
    private readonly PettyCommandsService _pettyCommandsService;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _permissionService.GetAllPermissionsAsync();
    }

    async Task RecordAudio()
    {
        try
        {
            if (!recorder.IsRecording) //Record button clicked
            {
                //recorder.StopRecordingOnSilence = TimeoutSwitch.IsToggled;

                //RecordButton.IsEnabled = false;
                PlayButton.IsEnabled = false;

                //start recording audio
                await recorder.StartRecording();
                //await recorder.StartRecordAsync(CultureInfo.CurrentCulture, this, _token.Token);

                RecordButton.Text = "Stop Recording";
                RecordButton.IsEnabled = true;

                RecordButton.Text = "Record";
                PlayButton.IsEnabled = true;
            }
            else //Stop button clicked
            {
                RecordButton.IsEnabled = false;

                //stop the recording...
                _token.Cancel();
                await recorder.StopRecording();
                //await recorder.StopRecordAsync();

                RecordButton.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            //blow up the app!
            throw ex;
        }
    }

    public void Report(string c)
    {
        if(!string.IsNullOrEmpty(c))
            lol.Text = c;
    }

    void Play_Clicked(object sender, EventArgs e)
    {
        PlayAudio();
    }

    void PlayAudio()
    {
        try
        {
            var filePath = recorder.FilePath;//.GetAudioFilePath();

            if (filePath != null)
            {
                PlayButton.IsEnabled = false;
                RecordButton.IsEnabled = false;

                player.Play(filePath);
            }
        }
        catch (Exception ex)
        {
            //blow up the app!
            throw ex;
        }
    }

    void Player_FinishedPlaying(object sender, EventArgs e)
    {
        PlayButton.IsEnabled = true;
        RecordButton.IsEnabled = true;
    }

}