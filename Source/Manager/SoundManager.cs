using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceMarines_TD.Source.Manager
{
    class SoundManager
    {
        private SoundEffect m_machineGun;
        private TimeSpan m_lastMachineGunSound;
        private const double m_machineGunPause = 180;

        private SoundEffect m_cannon;
        private TimeSpan m_lastCannonSound;
        private const double m_cannonPause = 300;

        private SoundEffect m_missileLaunch;
        private TimeSpan m_lastMissileSound;
        private const double m_missilePause = 250;

        private SoundEffect m_sling;
        private TimeSpan m_lastSlingSound;
        private const double m_slingPause = 150;

        private SoundEffect m_explosion;
        private TimeSpan m_lastExplosionSound;
        private const double m_explosionPause = 300;

        private SoundEffect m_cluster;
        private TimeSpan m_lastClusterSound;
        private const double m_clusterPause = 400;

        private SoundEffect m_death1;
        private SoundEffect m_death2;
        private TimeSpan m_lastDeathSound;
        private const double m_deathPause = 500;

        private SoundEffect m_lostLife;
        private TimeSpan m_lastLostLifeSound;
        private const double m_lostLifePause = 100;

        private SoundEffect m_placeTower;

        private SoundEffect m_error;

        private SoundEffect m_upgrade;

        private SoundEffect m_sell;

        private SoundEffect m_wave;

        private Song m_levelone;
        private Song m_leveltwo;
        private Song m_levelthree;

        private bool m_isLevelOnePlaying = false;
        private bool m_isLevelTwoPlaying = false;
        private bool m_isLevelThreePlaying = false;

        private Song m_mainMenu;
        private bool m_isMainMenuPlaying = false;

        public bool PlayMusic { get; set; }

        public SoundManager()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.15f;
            PlayMusic = true;
        }

        public void loadContent(ContentManager contentManager)
        {
            m_machineGun = contentManager.Load<SoundEffect>("sounds/machinegun");
            m_cannon = contentManager.Load<SoundEffect>("sounds/cannon");
            m_missileLaunch = contentManager.Load<SoundEffect>("sounds/missilelaunch");
            m_sling = contentManager.Load<SoundEffect>("sounds/slingshot");
            m_explosion = contentManager.Load<SoundEffect>("sounds/explosion");
            m_cluster = contentManager.Load<SoundEffect>("sounds/cluster");
            m_death1 = contentManager.Load<SoundEffect>("sounds/death1");
            m_death2 = contentManager.Load<SoundEffect>("sounds/death2");
            m_lostLife = contentManager.Load<SoundEffect>("sounds/lostlife");
            m_placeTower = contentManager.Load<SoundEffect>("sounds/placetower");
            m_error = contentManager.Load<SoundEffect>("sounds/error");
            m_upgrade = contentManager.Load<SoundEffect>("sounds/upgrade");
            m_sell = contentManager.Load<SoundEffect>("sounds/sell");
            m_wave = contentManager.Load<SoundEffect>("sounds/wave");

            m_levelone = contentManager.Load<Song>("songs/level1");
            m_leveltwo = contentManager.Load<Song>("songs/level2");
            m_levelthree = contentManager.Load<Song>("songs/level3");

            m_mainMenu = contentManager.Load<Song>("songs/mainmenu");
        }

        public void PlayBulletSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastMachineGunSound).TotalMilliseconds > m_machineGunPause)
            {
                m_machineGun.Play(0.25f, 0.0f, 0.0f);
                m_lastMachineGunSound = gameTime.TotalGameTime;
            }
        }

        public void PlayCannonSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastCannonSound).TotalMilliseconds > m_cannonPause)
            {
                m_cannon.Play(0.20f, 0.25f, 0.0f);
                m_lastCannonSound = gameTime.TotalGameTime;
            }
        }

        public void PlayMissileSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastMissileSound).TotalMilliseconds > m_missilePause)
            {
                m_missileLaunch.Play(0.20f, 0.0f, 0.0f);
                m_lastMissileSound = gameTime.TotalGameTime;
            }
        }

        public void PlaySlingSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastSlingSound).TotalMilliseconds > m_slingPause)
            {
                m_sling.Play(0.5f, 0.0f, 0.0f);
                m_lastSlingSound = gameTime.TotalGameTime;
            }
        }

        public void PlayExplosionSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastExplosionSound).TotalMilliseconds > m_explosionPause)
            {
                m_explosion.Play(0.20f, 0.0f, 0.0f);
                m_lastExplosionSound = gameTime.TotalGameTime;
            }
        }

        public void PlayClusterSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastClusterSound).TotalMilliseconds > m_clusterPause)
            {
                m_cluster.Play(0.35f, 0.0f, 0.0f);
                m_lastClusterSound = gameTime.TotalGameTime;
            }
        }
        
        public void PlayDeathSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastDeathSound).TotalMilliseconds > m_deathPause)
            {
                var rng = new Random();
                if(rng.Next() % 2 == 0)
                {
                    m_death1.Play(0.35f, 0.0f, 0.0f);
                }
                else
                {
                    m_death2.Play(0.35f, 0.0f, 0.0f);
                }
                m_lastDeathSound = gameTime.TotalGameTime;
            }
        }

        public void PlayLostLifeSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - m_lastLostLifeSound).TotalMilliseconds > m_lostLifePause)
            {
                m_lostLife.Play(0.5f, 0.0f, 0.0f);
                m_lastLostLifeSound = gameTime.TotalGameTime;
            }
        }

        public void PlayPlaceSound()
        {
            m_placeTower.Play(0.5f, 0.0f, 0.0f);
        }

        public void PlayErrorSound()
        {
            m_error.Play(0.25f, 0.0f, 0.0f);
        }

        public void PlayUpgradeSound()
        {
            m_upgrade.Play(0.20f, 0.0f, 0.0f);
        }

        public void PlaySellSound()
        {
            m_sell.Play(0.25f, 0.0f, 0.0f);
        }

        public void PlayWaveSound()
        {
            m_wave.Play(0.5f, 0.0f, 0.0f);
        }

        public void UpdateBackgroundMusic(int level)
        {
            if (PlayMusic)
            {
                if (level < 5 && !m_isLevelOnePlaying)
                {
                    StopMusic();
                    MediaPlayer.Play(m_levelone);
                    m_isLevelOnePlaying = true;
                }
                if (level > 5 && level < 10 && !m_isLevelTwoPlaying){
                    StopMusic();
                    MediaPlayer.Play(m_leveltwo);
                    m_isLevelTwoPlaying = true;
                }
                if (level > 10 && !m_isLevelThreePlaying)
                {
                    StopMusic();
                    MediaPlayer.Play(m_levelthree);
                    m_isLevelThreePlaying = true;
                }
            }
            else
            {
                StopMusic();
            }
        }

        public void UpdateMainMenuMusic()
        {
            if (PlayMusic)
            {
                if (!m_isMainMenuPlaying)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play(m_mainMenu);
                    m_isMainMenuPlaying = true;
                }
            }
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
            m_isLevelOnePlaying = false;
            m_isLevelTwoPlaying = false;
            m_isLevelThreePlaying = false;
            m_isMainMenuPlaying = false;
        }
    }
}
