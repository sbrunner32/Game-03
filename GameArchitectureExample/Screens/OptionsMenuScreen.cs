

namespace GameArchitectureExample.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
      
        private readonly MenuEntry _musicVolumeMenuEntry;
        private readonly MenuEntry _soundEffectVolumeMenuEntry;
        private static int _musicVolume = 5;
        private static int _soundEffectVolume = 5;




        public OptionsMenuScreen() : base("Options")
        {
            _musicVolumeMenuEntry = new MenuEntry(string.Empty);
            _soundEffectVolumeMenuEntry = new MenuEntry(string.Empty);
           
            

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _musicVolumeMenuEntry.Selected += MusicVolumeMenuEntrySelected;
            _soundEffectVolumeMenuEntry.Selected += SoundEffectVolumeMenuEntrySelected;

            back.Selected += OnCancel;

            MenuEntries.Add(_musicVolumeMenuEntry);
            MenuEntries.Add(_soundEffectVolumeMenuEntry);

            MenuEntries.Add(back);
        }

 
        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _musicVolumeMenuEntry.Text= $"Music Volume: { _musicVolume.ToString()}";
            _soundEffectVolumeMenuEntry.Text = $"Sound Effect Volume: { _soundEffectVolume.ToString()}";

        }       

        private void MusicVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (_musicVolume >= 10)
                _musicVolume = 0;
            else
                _musicVolume++;
            SetMenuEntryText();
        }
        private void SoundEffectVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (_soundEffectVolume >= 10)
                _soundEffectVolume = 0;
            else
                _soundEffectVolume++;
            SetMenuEntryText();
        }
    }
}
