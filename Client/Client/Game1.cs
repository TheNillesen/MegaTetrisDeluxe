using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private TextField textField;
        private Text text;

        //For the different times the textfield class is used, and indicates what it's used for.
        public bool hosting;
        public bool connecting;

        public Song backGroundMusic;
        public List<GameObject> gameObjects;     
        public GameMap gameMap;
        public GameObject player;
        public Vector2 playerStartPosition;
        public string IPForServerConnection;


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

            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            Gameworld.Instance.IsMouseVisible = true; //Makes the mouse visible within the gamewindow
//#if DEBUG
//            GameClient gc = new GameClient();
//            new System.Threading.Thread(() => gc.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 6666)).Start();
//#endif
            gameObjects = new List<GameObject>();
            gameMap = new GameMap(20f, 20f, 1000, 700, new Vector2(0, 0));
            gameMap.Borders(Color.White);

            playerStartPosition = new Vector2(gameMap.map.GetLength(0) / 2, 4);
            connecting = false;
            textField = new TextField("Border", gameMap.gameAreaWidth / 2, gameMap.gameAreaHeight / 2, new Vector2(5, 1));
            textField.LoadContent(this.Content);
            text = new Text(Color.White, 20, new Vector2(20, - 300));
            text.LoadContent(this.Content);

            //Test player
            CreatPlayer();
        }

        public GameObject GetGameobject(Predicate<GameObject> filter)
        {
            return gameObjects.Find(filter);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backGroundMusic = Content.Load<Song>("Original Tetris Theme");
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
            if (connecting || hosting)
                textField.Update();
            text.Update();
            
            base.Update(gameTime);

            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Draw(spriteBatch);
            player.Draw(spriteBatch);
            if (connecting || hosting)
                textField.Draw(spriteBatch);
            text.Draw(spriteBatch);

           

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void startServer(int port, int gridWidth, int gridHeight, long tickCount)
        {
            Process process = new Process();

            process.StartInfo.Arguments = $"port:{port.ToString()} width:{gridWidth} height:{gridHeight} tickCount:{tickCount}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "Server.exe";

            process.Start();
        }

        public void OnTick()
        {
            foreach (GameObject go in gameObjects)
                go.OnTick();
        }

        public void AddGameObject(GameObject go)
        {
            gameObjects.Add(go);
        }

        public void RemoveObject(GameObject go)
        {
            gameObjects.Remove(go);
        }

        public void CreatPlayer()
        {
            player = new GameObject();
            player.AddComponent(new Spriterendere(player, "GreyToneBlock", 1f));
            player.AddComponent(new Transform(player, playerStartPosition));
            player.AddComponent(new PlayerController(player));
            player.LoadContent(this.Content);
            gameObjects.Add(player);
        }
    }
}
