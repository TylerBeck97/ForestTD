using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Input;
using SpaceMarines_TD.Source.Views;

namespace SpaceMarines_TD.Source
{
    public class GameLoop : Game
    {
        private GraphicsDeviceManager m_graphics;
        private IGameState m_currentState;
        private GameStateEnum m_nextStateEnum = GameStateEnum.GamePlay; //TODO: Change to MainMenu
        private Dictionary<GameStateEnum, IGameState> m_states;
        private SettingsManager m_settingsManager;
        private HighScoreManager m_highScoreManager;

        public GameLoop()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            m_settingsManager = new SettingsManager();
            m_highScoreManager = new HighScoreManager();
        }

        protected override void Initialize()
        {
            m_settingsManager.Load();
            m_highScoreManager.Load();

            m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;

            m_graphics.ApplyChanges();

            m_states = new Dictionary<GameStateEnum, IGameState>(); 
            m_states.Add(GameStateEnum.MainMenu, new MainMenuView());
            m_states.Add(GameStateEnum.GamePlay, new GamePlayView(m_settingsManager));
            m_states.Add(GameStateEnum.HighScores, new HighScoreView(m_highScoreManager));
            m_states.Add(GameStateEnum.Controls, new ControlsView(m_settingsManager));
            m_states.Add(GameStateEnum.Credits, new CreditsView());

            m_currentState = m_states[GameStateEnum.MainMenu]; // TODO change back to MainMenu

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (var item in m_states)
            {
                item.Value.initialize(GraphicsDevice, m_graphics);
                item.Value.loadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            m_nextStateEnum = m_currentState.processInput(gameTime);

            if (m_nextStateEnum == GameStateEnum.Exit) Exit();

            m_currentState.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_currentState.render(gameTime);

            m_currentState = m_states[m_nextStateEnum];

            base.Draw(gameTime);
        }
    }
}
