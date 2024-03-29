using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Input;
using SpaceMarines_TD.Source.Manager;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Views
{
    class GamePlayView : GameStateView
    {
        public const int TowerSize = 99;
        public const int ProjectileSize = 50;

        private SettingsManager m_settings;
        private HighScoreManager m_highscoreManager;

        private KeyboardInput m_inputKeyboard;
        private MouseInput m_inputMouse;

        private CreepManager m_creepManager;
        private ProjectileManager m_projectileManager;
        private TowerManager m_towerManager;

        private Texture2D m_backgroundTexture2D;
        private Rectangle m_battleFieldRectangle = new Rectangle(200, 0, 1100, 1100);

        private UIOverlay m_uiOverlay;

        private SpriteSheet m_spriteSheet;

        private GameStateManager m_gameStateManager;

        private ParticleManager m_particleManager;
        private SoundManager m_soundManager;

        private bool m_waitForKeyRelease;

        public GamePlayView(SettingsManager settings, HighScoreManager highScoreManager, SoundManager soundManager)
        {

            m_settings = settings;
            m_settings.SettingsChanged += OnSettingsChanged;

            m_highscoreManager = highScoreManager;

            m_inputKeyboard = new KeyboardInput();
            m_inputMouse = new MouseInput();

            m_gameStateManager = new GameStateManager();

            m_particleManager = new ParticleManager();
            m_soundManager = soundManager;
            m_creepManager = new CreepManager(m_gameStateManager, m_particleManager, m_soundManager);
            m_projectileManager = new ProjectileManager(m_gameStateManager, m_particleManager, m_soundManager);
            m_towerManager = new TowerManager(m_gameStateManager, m_particleManager, m_soundManager, m_inputMouse);
            
            m_uiOverlay = new UIOverlay(new Rectangle(1480, 0, 440, 1080), m_gameStateManager, m_towerManager);

            m_waitForKeyRelease = true;
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            RegisterKeyBindings();
        }

        private void RegisterKeyBindings()
        {
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Air), true, (time, value) => m_towerManager.SetPlaceTowerType(TowerType.Air));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Ground), true, (time, value) => m_towerManager.SetPlaceTowerType(TowerType.Bullet));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Mixed), true, (time, value) => m_towerManager.SetPlaceTowerType(TowerType.Mixed));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Bomb), true, (time, value) => m_towerManager.SetPlaceTowerType(TowerType.Bomb));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.SellTower), true, SellTower);
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Upgrade), true, UpgradeTower);
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.StartLevel), true, StartLevel);
            m_inputKeyboard.registerCommand(Keys.F1, true, (time, value) => m_soundManager.PlayMusic = !m_soundManager.PlayMusic);
            m_inputKeyboard.registerCommand(Keys.F2, true, RestartLevel);
            m_inputKeyboard.registerCommand(Keys.F3, true, (time, value) => m_gameStateManager.Money += 1000);
            m_inputKeyboard.registerCommand(Keys.F4, true, (time, value) => m_gameStateManager.Level += 1);
        }
        
        private void StartLevel(GameTime gameTime, float value)
        {
            if (!m_gameStateManager.isLevelActive && !m_gameStateManager.isGameOver)
            {
                m_gameStateManager.isLevelActive = true;
                m_gameStateManager.Wave = 0;
                m_gameStateManager.Level++;
            }
        }

        private void RestartLevel(GameTime gameTime, float value)
        {
             m_highscoreManager.SubmitScore(m_gameStateManager.Score);
             m_gameStateManager.Reset();
        }

        private void UpgradeTower(GameTime gametime, float value)
        {
            if (m_towerManager.SelectedTower != null)
            {
                m_towerManager.UpgradeTower(m_towerManager.SelectedTower);
            }
        }

        private void SellTower(GameTime gametime, float value)
        {
            if (m_towerManager.SelectedTower != null)
            {
                m_towerManager.SellTower(m_towerManager.SelectedTower);
                m_towerManager.SelectedTower = null;
            }
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_backgroundTexture2D = contentManager.Load<Texture2D>("images/FullBackground");

            var spriteTexture = contentManager.Load<Texture2D>("images/SpriteSheet");
            m_spriteSheet = new SpriteSheet(spriteTexture);

            m_towerManager.loadContent(contentManager, m_spriteSheet);
            m_creepManager.loadContent(contentManager, m_spriteSheet);
            m_projectileManager.loadContent(contentManager, m_spriteSheet);
            m_particleManager.loadContent(m_spriteSheet.Texture);
            m_soundManager.loadContent(contentManager);

            m_uiOverlay.Intialize(contentManager, m_settings, m_spriteSheet);
            
            RegisterKeyBindings();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputMouse.Update(gameTime, m_scalingMatrix);
            m_inputKeyboard.Update(gameTime);

            var handled = m_uiOverlay.HandleInput(m_inputMouse);

            if (m_inputMouse.Clicked && !handled)
            {
                m_towerManager.OnClick(m_inputMouse.Position);
            }

            var state = Keyboard.GetState();
            if (!m_waitForKeyRelease)
            {
                if (state.IsKeyDown(Keys.Escape))
                {
                    if (m_gameStateManager.isGameOver || m_towerManager.GetPlaceTowerType() == null)
                    {
                        RestartLevel(gameTime, 1);
                        m_soundManager.StopMusic();
                        return GameStateEnum.MainMenu;
                    }
                    else
                    {
                        m_towerManager.SetPlaceTowerType(null);
                    }
                    m_waitForKeyRelease = true;
                }
            }
            else if (state.IsKeyUp(Keys.Escape)) m_waitForKeyRelease = false;

            return GameStateEnum.GamePlay;
        }

        public override void update(GameTime gameTime)
        {
            m_towerManager.Update(gameTime, m_battleFieldRectangle);
            m_creepManager.Update(gameTime, m_battleFieldRectangle);
            m_projectileManager.Update(gameTime, m_battleFieldRectangle);
            m_particleManager.Update(gameTime);
            m_soundManager.UpdateBackgroundMusic(m_gameStateManager.Level);
            m_uiOverlay.Update(gameTime);
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(transformMatrix: m_scalingMatrix);

            m_spriteBatch.Draw(m_backgroundTexture2D, new Rectangle(0, 0, 1920, 1080), Color.White);

            m_towerManager.Draw(gameTime, m_spriteBatch);
            m_creepManager.Draw(m_spriteBatch);
            m_projectileManager.Draw(m_spriteBatch);
            m_particleManager.Draw(m_spriteBatch);

            m_uiOverlay.Draw(m_spriteBatch);

            m_spriteBatch.End();
        }
    }
}
