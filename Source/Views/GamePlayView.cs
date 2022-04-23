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
using TestGame.Services;

namespace SpaceMarines_TD.Source.Views
{
    class GamePlayView : GameStateView
    {
        public const int TowerSize = 99;
        public const int CreepSize = 50;
        public const int ProjectileSize = 50;

        private SettingsManager m_settings;

        private KeyboardInput m_inputKeyboard;
        private MouseInput m_inputMouse;

        private CreepManager m_creepManager;
        private ProjectileManager m_projectileManager;
        private TowerManager m_towerManager;

        private Texture2D m_backgroundTexture2D;
        private Rectangle m_backgroundRectangle = new Rectangle(200, 0, 1050, 1050);

        private SideBar m_sideBar;

        private SpriteSheet m_spriteSheet;

        private GameStateManager m_gameStateManager;

        // DEBUG
        private DrawingService _drawingService;
        private DebugService _debugService;
        private DebugText m_fpsText;

        public GamePlayView(SettingsManager settings)
        {
            m_settings = settings;
            m_settings.SettingsChanged += OnSettingsChanged;

            m_gameStateManager = new GameStateManager();

            m_creepManager = new CreepManager(m_gameStateManager);
            m_projectileManager = new ProjectileManager(m_gameStateManager);
            m_towerManager = new TowerManager(m_gameStateManager);

            m_sideBar = new SideBar(new Rectangle(1480, 0, 440, 1080), m_gameStateManager, m_towerManager);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            RegisterKeyBindings();
        }

        private void RegisterKeyBindings()
        {
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Air), true, (time, value) => SetPlaceTowerType(TowerType.Air));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Ground), true, (time, value) => SetPlaceTowerType(TowerType.Bullet));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Mixed), true, (time, value) => SetPlaceTowerType(TowerType.Mixed));
            m_inputKeyboard.registerCommand(Enum.Parse<Keys>(m_settings.Bindings.Bomb), true, (time, value) => SetPlaceTowerType(TowerType.Bomb));
        }

        private void SetPlaceTowerType(TowerType type)
        {
            // Check to make sure the player has enough money.
            if (m_gameStateManager.Money >= TowerManager.GetCost(type))
            {
                m_towerManager.SetPlaceTowerType(type);
            }
            else
            {
                // TODO Play a sound?
            }
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_backgroundTexture2D = contentManager.Load<Texture2D>("Background");

            var spriteTexture = contentManager.Load<Texture2D>("SpriteSheet");
            m_spriteSheet = new SpriteSheet(spriteTexture);

            // DEBUG
            _drawingService = new DrawingService(m_graphics);
            _drawingService.Load();

            _debugService = new DebugService(_drawingService);
            _debugService.LoadContent(contentManager);

            _debugService.DebugOverlayVisible = true;
            //m_fpsText = _debugService.CreateDebugText();

            m_towerManager.loadContent(contentManager, m_spriteSheet);
            m_creepManager.loadContent(contentManager, m_spriteSheet, _debugService);
            m_projectileManager.loadContent(contentManager, m_spriteSheet);

            m_inputKeyboard = new KeyboardInput();
            m_inputMouse = new MouseInput();

            m_sideBar.Intialize(contentManager);

            RegisterKeyBindings();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputMouse.Update(gameTime);
            m_inputKeyboard.Update(gameTime);

            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
            {
                m_towerManager.SetPlaceTowerType(null);
            }

            var handled = m_sideBar.HandleInput(m_inputMouse);

            if (m_inputMouse.Clicked && !handled)
            {
                m_towerManager.OnClick(m_inputMouse.Position);
            }

            return GameStateEnum.GamePlay;
        }

        public override void update(GameTime gameTime)
        {
            m_towerManager.Update(gameTime, m_backgroundRectangle, CreepSize);
            m_creepManager.Update(gameTime, m_backgroundRectangle, CreepSize);
            m_projectileManager.Update(gameTime, m_backgroundRectangle);
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            m_spriteBatch.Draw(m_backgroundTexture2D, m_backgroundRectangle, Color.White);
            m_sideBar.Draw(m_spriteBatch);

            m_towerManager.Draw(gameTime, m_spriteBatch);
            m_creepManager.Draw(m_spriteBatch);
            m_projectileManager.Draw(m_spriteBatch);

            m_spriteBatch.End();

            // m_fpsText.Message = $"FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds:0.##}";
            _debugService.Draw(gameTime);
        }
    }
}
