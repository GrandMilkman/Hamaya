using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midi;
using kursach2018;

namespace Game1
{
    //TODO
    public class Level3
    {
        public class Player
        {
            private List<Texture2D> texture;
            private Vector2 position;
            public int health = 6;
            private int currentFrame = 0;
            private int count = 0;
            private Random rand;
            private PianoKeyboard synth;
            private Pitch key1;
            private Pitch key2;

            private Pitch[] keys = {
                Pitch.C3, Pitch.CSharp3, Pitch.D3, Pitch.DSharp3, Pitch.E3, Pitch.F3, Pitch.FSharp3, Pitch.G3, Pitch.GSharp3, Pitch.A3, Pitch.ASharp3, Pitch.B3,
                Pitch.C4, Pitch.CSharp4, Pitch.D4, Pitch.DSharp4, Pitch.E4, Pitch.F4, Pitch.FSharp4, Pitch.G4, Pitch.GSharp4, Pitch.A4, Pitch.ASharp4, Pitch.B4,
                Pitch.C5, Pitch.CSharp5, Pitch.D5, Pitch.DSharp5, Pitch.E5, Pitch.F5, Pitch.FSharp5, Pitch.G5, Pitch.GSharp5, Pitch.A5, Pitch.ASharp5, Pitch.B5,
                Pitch.C6, Pitch.CSharp6, Pitch.D6, Pitch.DSharp6, Pitch.E6, Pitch.F6, Pitch.FSharp6, Pitch.G6, Pitch.GSharp6, Pitch.A6, Pitch.ASharp6, Pitch.B6,
            };

            public Player()
            {
                //synth = new PianoKeyboard();
                texture = new List<Texture2D>();
                position = new Vector2(100, 350);
                rand = new Random();

                int k = rand.Next(0, keys.Length - 2);
                key1 = (Pitch)keys[k];
                key2 = (Pitch)keys[k + 1];
            }

            public void KeysUpdate()
            {
                int k = rand.Next(0, keys.Length - 2);
                key1 = (Pitch)keys[k];
                key2 = (Pitch)keys[k + 1];
                count = 0;
            }

            public void GiveFork(ref List<Worker> workers)
            {
                foreach (var item in workers)
                {
                    if (item.position.X >= 200 && item.position.X <= 300)
                    {
                        item.fork++;
                    }
                }
            }

            public void GiveRake(ref List<Worker> workers)
            {
                foreach (var item in workers)
                {
                    if (item.position.X >= 200 && item.position.X <= 300)
                    {
                        item.rake++;
                    }
                }
            }

            private void KeyboardControl(ref List<Worker> workers)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyUp(Keys.F) && currentFrame == 1)
                {
                    GiveFork(ref workers);
                    count++;
                    currentFrame = 0;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.F) && Keyboard.GetState().IsKeyUp(Keys.D) && currentFrame == 0)
                {
                    GiveRake(ref workers);
                    count++;
                    currentFrame = 1;
                }
            }

            private void SynthControl(ref List<Worker> workers)
            {
                if (synth != null)
                {
                    //TODO
                    if (synth.KeyDown((int)key1) && currentFrame == 1)
                    {
                        GiveFork(ref workers);
                        count++;
                        currentFrame = 0;
                        synth.Device.RemoveSignal();
                    }
                    else if (synth.KeyDown((int)key2) && currentFrame == 0)
                    {
                        GiveRake(ref workers);
                        count++;
                        currentFrame = 1;
                        synth.Device.RemoveSignal();
                    }
                }
            }

            public void Update(ref List<Worker> workers)
            {
                if (count >= workers.Count * 2)
                {
                    KeysUpdate();
                }

                //SynthControl(ref workers);
                KeyboardControl(ref workers);
            }

            public void SetTexture(Texture2D texture)
            {
                this.texture.Add(texture);
            }

            public Texture2D GetTexture()
            {
                return texture[currentFrame];
            }

            public Vector2 GetPosition()
            {
                return position;
            }

            public int GetHealth()
            {
                return health;
            }

            public int GetCount()
            {
                return count;
            }

            public string GetKey1()
            {
                return key1.ToString();
            }

            public string GetKey2()
            {
                return key2.ToString();
            }
        }

        public class Worker
        {
            public List<Texture2D> texture;
            public Vector2 position;
            public float currentFrame = 0;
            private float speedFrame = 0.1f;
            public float speedMovement = 2f;
            public int fork = 0;
            public int rake = 0;
            private bool attack = true;

            public Worker(Vector2 position)
            {
                this.position = position;
                texture = new List<Texture2D>();
            }

            public void SetTexture(Texture2D texture)
            {
                this.texture.Add(texture);
            }

            public Texture2D GetTexture()
            {
                return texture[(int)currentFrame];
            }

            public void Move(float x, float y)
            {
                position.X += x;
                position.Y += y;
            }

            private void SetPosition(Vector2 position)
            {
                this.position = position;
            }

            public Vector2 GetPosition()
            {
                return position;
            }

            private void FrameUpdate()
            {
                currentFrame += speedFrame;

                if (currentFrame >= texture.Count)
                {
                    currentFrame = 0;
                }
            }

            private void CheckItem(ref Player player)
            {
                if (attack == true && (fork == 0 || fork > 1) && (rake == 0 || rake > 1) && position.X < -100)
                {
                    player.health--;
                    attack = false;
                }
            }

            private void CheckBound()
            {
                if (position.X + texture[0].Width < 0)
                {
                    SetPosition(new Vector2(1800, 350));
                    fork = 0;
                    rake = 0;
                    attack = true;
                }
            }

            public void Update(ref Player player)
            {
                FrameUpdate();
                CheckItem(ref player);
                CheckBound();
                Move(-speedMovement, 0);
            }
        }

        SpriteFont font;
        Player player;
        List<Worker> workers;
        List<Texture2D> flowers;
        List<Texture2D> clouds;
        Texture2D grass;

        public void Init()
        {
            flowers = new List<Texture2D>();
            clouds = new List<Texture2D>();
            player = new Player();

            workers = new List<Worker>();
            workers.Add(new Worker(new Vector2(1000, 350)));
            workers.Add(new Worker(new Vector2(1200, 350)));
            workers.Add(new Worker(new Vector2(1400, 350)));
            workers.Add(new Worker(new Vector2(1600, 350)));
            workers.Add(new Worker(new Vector2(1800, 350)));
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            font = Content.Load<SpriteFont>("font");

            flowers.Add(Content.Load<Texture2D>("9"));
            flowers.Add(Content.Load<Texture2D>("9"));
            flowers.Add(Content.Load<Texture2D>("9"));
            flowers.Add(Content.Load<Texture2D>("10"));
            flowers.Add(Content.Load<Texture2D>("10"));

            clouds.Add(Content.Load<Texture2D>("cloud1"));
            clouds.Add(Content.Load<Texture2D>("cloud1"));
            clouds.Add(Content.Load<Texture2D>("cloud2"));

            grass = Content.Load<Texture2D>("grass");

            player.SetTexture(Content.Load<Texture2D>("5"));
            player.SetTexture(Content.Load<Texture2D>("6"));

            foreach (var item in workers)
            {
                item.SetTexture(Content.Load<Texture2D>("2"));
                item.SetTexture(Content.Load<Texture2D>("3"));
            }
        }

        public void Update()
        {
            player.Update(ref workers);

            foreach (var item in workers)
            {
                item.Update(ref player);
            }
        }

        public void Draw(ref SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(flowers[0], new Vector2(180, 650), Color.White);
            spriteBatch.Draw(flowers[2], new Vector2(900, 650), Color.White);

            spriteBatch.Draw(grass, new Vector2(0, 480), color: Color.White, scale: new Vector2(2f, 2f));

            foreach (var item in workers)
            {
                spriteBatch.Draw(item.GetTexture(), item.GetPosition(), color: Color.White, effects: SpriteEffects.FlipHorizontally);
            }

            spriteBatch.Draw(player.GetTexture(), player.GetPosition(), Color.White);

            spriteBatch.Draw(flowers[1], new Vector2(400, 700), Color.White);
            spriteBatch.Draw(flowers[3], new Vector2(-50, 600), Color.White);
            spriteBatch.Draw(flowers[4], new Vector2(1200, 600), Color.White);

            spriteBatch.Draw(clouds[0], new Vector2(100, 150), Color.White);
            spriteBatch.Draw(clouds[1], new Vector2(1000, 230), Color.White);
            spriteBatch.Draw(clouds[2], new Vector2(600, 100), Color.White);

            spriteBatch.DrawString(font, player.GetKey1(), new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - 200, 100), player.GetCount() % 2 == 0 ? Color.Black : Color.Yellow);
            spriteBatch.DrawString(font, player.GetKey2(), new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 + 140, 100), player.GetCount() % 2 != 0 ? Color.Black : Color.Yellow);
            spriteBatch.DrawString(font, player.GetHealth() > 0 ? player.GetHealth().ToString() : "YOU DIED", new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - 20, 20), Color.Red);

            spriteBatch.End();
        }
    }
}
