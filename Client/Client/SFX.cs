using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace Client
{
    class SFX
    {
        public bool isPlaying;
        private Song backGround;
        
       
        public void PlayMusic()
        {
            MediaPlayer.Play(Gameworld.Instance.backGroundMusic);
        }
        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }
        public void MusicVolumeDown()
        {
            MediaPlayer.Volume -= 0.01f;
        }
        public void MusicVolumeUp()
        {
            MediaPlayer.Volume += 0.01f;
        }

    }
}
