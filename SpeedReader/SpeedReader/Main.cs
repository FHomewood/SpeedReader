using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace SpeedReader
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont headingFont, paragraphFont, buttonFont, readingFont;
        MouseState MState, OldMState;
        KeyboardState KState, OldKState;
        Texture2D pixel;
        int height, width;

        string screenStage = "MainMenu";
        string file, book, word = "Are you ready?";
        string[] fileList, progressList, Words;
        string[][] WordList;

        bool Reading;
        int scrollHeight, wpm, elapsedFrames, wordNo;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            OldMState = Mouse.GetState();
            OldKState = Keyboard.GetState();
            UpdateFiles();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = Content.Load<Texture2D>("Pixel");
            headingFont = Content.Load<SpriteFont>("Heading");
            paragraphFont = Content.Load<SpriteFont>("Paragraph");
            buttonFont = Content.Load<SpriteFont>("Buttons");
            readingFont = Content.Load<SpriteFont>("Reading");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MState = Mouse.GetState();
            KState = Keyboard.GetState();
            if (screenStage == "MainMenu")
            {
                if (MState.LeftButton == ButtonState.Released && OldMState.LeftButton == ButtonState.Pressed && new Rectangle((int)(0.42 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)).Contains(MState.Position)) { screenStage = "StartMenu"; }
                if (MState.LeftButton == ButtonState.Released && OldMState.LeftButton == ButtonState.Pressed && new Rectangle((int)(0.22 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)).Contains(MState.Position)) { screenStage = "OptionsMenu"; }
                if (MState.LeftButton == ButtonState.Released && OldMState.LeftButton == ButtonState.Pressed && new Rectangle((int)(0.62 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)).Contains(MState.Position)) { screenStage = "HelpMenu"; }
            }
            if (screenStage == "StartMenu")
            {
                if (MState.ScrollWheelValue != OldMState.ScrollWheelValue) { scrollHeight += (MState.ScrollWheelValue - OldMState.ScrollWheelValue) / 6; }
                if (scrollHeight > 0) { scrollHeight = 0; }
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (MState.LeftButton == ButtonState.Pressed && OldMState.LeftButton == ButtonState.Released && new Rectangle((int)(0.3 * width), (int)((0.3 + 0.1 * i) * height + scrollHeight), (int)(0.6 * width), (int)(0.1 * height)).Contains(MState.Position))
                    {
                        file = fileList[i];
                        screenStage = "Reading";
                        Words = WordList[i];

                        try
                        { 
                            StreamReader MemoryReader = new StreamReader(Directory.GetCurrentDirectory() + "\\memory\\" + Path.GetFileName(file));
                            wordNo = int.Parse(MemoryReader.ReadToEnd());
                            MemoryReader.Close();
                        }
                        catch { }

                    }
                }
            }
            if (screenStage == "Reading")
            {
                if (KState.IsKeyDown(Keys.Space) && OldKState.IsKeyUp(Keys.Space)) { wpm = 0; }
                if (MState.ScrollWheelValue != OldMState.ScrollWheelValue) { Reading = true; wpm += (MState.ScrollWheelValue - OldMState.ScrollWheelValue) * 5 / 24; }
                if (new Rectangle(0, 0, width, (int)(0.02 * height)).Contains(MState.Position) && MState.LeftButton == ButtonState.Pressed) { wordNo = Words.Length * MState.X / width; wpm = 0; }
                if (wpm != 0)
                {
                    if (Reading && elapsedFrames % (3600 / wpm) == 0)
                    {
                        if (wpm > 0)
                            wordNo++;
                        else
                            wordNo--;
                        if (wpm > 3599)
                        {
                            wpm = 3599;
                        }
                        if (wordNo < 1)
                        {
                            wordNo = 0;
                            word = "Are you ready?";
                        }
                        else if (wordNo > Words.Length)
                        {
                            wordNo = Words.Length + 1;
                            word = "End of Book";
                        }
                        else
                            word = Words[wordNo - 1];
                    }
                }
                if (elapsedFrames % 1800 == 0)
                {
                    StreamWriter Writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\memory\\" + Path.GetFileName(file), false);
                    Writer.WriteLine(wordNo.ToString());
                    Writer.Close();
                }
            }
            elapsedFrames++;
            OldMState = MState;
            OldKState = KState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(44, 133, 193, 255));
            spriteBatch.Begin();
            if (screenStage == "MainMenu")
            {
                spriteBatch.Draw(pixel, new Rectangle((int)(0.22 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)), Color.FromNonPremultiplied(100, 178, 229, 255));
                spriteBatch.Draw(pixel, new Rectangle((int)(0.42 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)), Color.FromNonPremultiplied(100, 178, 229, 255));
                spriteBatch.Draw(pixel, new Rectangle((int)(0.62 * width), (int)(0.60 * height), (int)(0.16 * width), (int)(0.08 * height)), Color.FromNonPremultiplied(100, 178, 229, 255));

                spriteBatch.DrawString(headingFont, "Speed Reader", new Vector2(0.5f * width, 0.4f * height) - headingFont.MeasureString("Speed Reader") / 2, Color.FromNonPremultiplied(255,255,255,200));
                spriteBatch.DrawString(buttonFont, "Options", new Vector2(0.3f * width, 0.64f * height) - buttonFont.MeasureString("Options") / 2, Color.FromNonPremultiplied(255, 255, 255, 200));
                spriteBatch.DrawString(buttonFont, "Start", new Vector2(0.5f * width, 0.64f * height) - buttonFont.MeasureString("Start") / 2, Color.FromNonPremultiplied(255, 255, 255, 200));
                spriteBatch.DrawString(buttonFont, "Exit", new Vector2(0.7f * width, 0.64f * height) - buttonFont.MeasureString("Exit") / 2, Color.FromNonPremultiplied(255, 255, 255, 200));
            }
            if (screenStage == "StartMenu")
            {
                for (int i = 0; i < fileList.Length; i++)
                {
                    spriteBatch.Draw(pixel, new Rectangle((int)(0.3 * width), (int)((0.3 + 0.1 * i) * height + scrollHeight), (int)(0.6 * width), (int)(0.1 * height)), Color.FromNonPremultiplied(84, 171, 229, 255));
                    try
                    {
                        spriteBatch.Draw(pixel, new Rectangle((int)(0.3 * width), (int)((0.3 + 0.1 * i) * height + scrollHeight), (int)(0.6 * width * int.Parse(progressList[i]) / WordList[i].Length), (int)(0.1 * height)), Color.FromNonPremultiplied(0,0,0,64));
                    }
                    catch { }
                    spriteBatch.Draw(pixel, new Rectangle((int)(0.3 * width), (int)((0.3 + 0.1 * i) * height + scrollHeight), (int)(0.6 * width), (int)(0.01 * height)), Color.FromNonPremultiplied(30, 120, 180, 255));

                    spriteBatch.DrawString(paragraphFont, Path.GetFileNameWithoutExtension(fileList[i]), new Vector2(0.32f * width, (float)(0.33 + 0.1 * i) * height + scrollHeight) - new Vector2(0, paragraphFont.MeasureString(Path.GetFileNameWithoutExtension(fileList[i])).Y) / 2, Color.White);
                    try
                    {
                        spriteBatch.DrawString(paragraphFont, (100 * int.Parse(progressList[i]) / WordList[i].Length).ToString() + "%", new Vector2(0.89f * width, (float)(0.38 + 0.1 * i) * height + scrollHeight) - new Vector2(paragraphFont.MeasureString((100 * int.Parse(progressList[i]) / WordList[i].Length).ToString() + "%").X, paragraphFont.MeasureString((100 * int.Parse(progressList[i]) / WordList[i].Length).ToString() + "%").Y / 2), Color.White);
                    }
                    catch { spriteBatch.DrawString(paragraphFont, "0%", new Vector2(0.89f * width, (float)(0.38 + 0.1 * i) * height + scrollHeight) - new Vector2(paragraphFont.MeasureString("0%").X, paragraphFont.MeasureString("0%").Y / 2), Color.White); }
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, width, (int)(0.3 * height)), Color.FromNonPremultiplied(44, 133, 193, 255));
                    spriteBatch.Draw(pixel, new Rectangle(0, (int)(0.7 * height), width, (int)(0.3 * height)), Color.FromNonPremultiplied(44, 133, 193, 255));
                    spriteBatch.DrawString(headingFont, "Speed Reader", new Vector2(0.1f * width, 0.1f * height), Color.FromNonPremultiplied(255, 255, 255, 200));
                }
            }
            if (screenStage == "Reading")
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, width, height), Color.Black);
                spriteBatch.DrawString(readingFont, word, new Vector2(width / 2, height / 2) - readingFont.MeasureString(word) / 2, Color.DimGray);
                spriteBatch.DrawString(paragraphFont, "WPM: " + wpm.ToString(), new Vector2(0.1f * width, 0.1f * height) - paragraphFont.MeasureString("WPM: " + wpm.ToString()) / 2, Color.DarkGray);
                spriteBatch.DrawString(paragraphFont, "Word: " + wordNo.ToString(), new Vector2(0.1f * width, 0.13f * height) - paragraphFont.MeasureString("Word: " + wordNo.ToString()) / 2, Color.DarkGray);
                spriteBatch.Draw(pixel, new Rectangle(0, 0, (int)(width * wordNo / Words.Length), (int)(0.02 * height)), Color.DarkGray);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    private void UpdateFiles()
    {
            fileList = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\texts");
            progressList = new string[fileList.Length];
            WordList = new string[fileList.Length][];
            foreach (string F in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\memory"))
            {
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (Path.GetFileName(F) == Path.GetFileName(fileList[i]))
                    {
                        progressList[i] = new StreamReader(F).ReadToEnd();
                        new StreamReader(F).Close();

                    }
                    WordList[i] = new StreamReader(fileList[i]).ReadToEnd().Split(' ');
                    new StreamReader(fileList[i]).Close();
                }
            }
        }
    }
}
