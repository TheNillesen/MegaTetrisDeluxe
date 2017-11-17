using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Anders.Vestergaard;
using Andreas.Gade;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class Gameworld : Game
    {
        private static Gameworld instance;
        public static Gameworld Instance
        {
            get
            {
                return instance == null ? instance = new Gameworld() : instance;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private bool hasRun = false;
        private DateTime startup = DateTime.Now;
        private List<GameObject> gameObjects;     

        public GameMap gameMap;
        public GameObject player;

        private Gameworld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

#if DEBUG
            GameClient gc = new GameClient();
            new System.Threading.Thread(() => gc.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 6666)).Start();
#endif
            gameObjects = new List<GameObject>();
            gameMap = new GameMap(20, 20, 100, 100, new Vector2(0, 0));

            //Test player
            player = new GameObject();
            player.AddComponent(new Spriterendere(player, "GreyToneBlock.png", 1f));
            player.AddComponent(new Transform(player, new Vector2(5, 5)));
            player.AddComponent(new PlayerController(player));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.LoadContent(this.Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Update();
            player.Update();

            base.Update(gameTime);

            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Draw(spriteBatch);
            player.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        public static void startServer(int port)
        {
            Process process = new Process();

            process.StartInfo.Arguments = $"port:{port.ToString()}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "Server.exe";

            process.Start();
        }
    }
}
