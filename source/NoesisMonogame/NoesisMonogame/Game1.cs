using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace NoesisMonogame
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Noesis.View _guiView;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
        }

        private static void LogGUIMessage(Noesis.LogLevel level, string channel, string message)
        {
            Console.WriteLine($"[{level}] {message}");
        }
        
        protected override void Initialize()
        {
            Noesis.Log.SetLogCallback(LogGUIMessage);
            Noesis.GUI.Init();
            Noesis.GUI.SetLicense(NoesisLicense.Name, NoesisLicense.Key);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var directXDevice = (SharpDX.Direct3D11.Device) GraphicsDevice.Handle;
            var deviceContext = directXDevice.ImmediateContext;
            var guiDevice = new Noesis.RenderDeviceD3D11(deviceContext.NativePointer, sRGB: false);

            NoesisApp.Application.SetThemeProviders();
            Noesis.GUI.LoadApplicationResources("Theme/NoesisTheme.DarkBlue.xaml");

            Noesis.Grid xaml = (Noesis.Grid)Noesis.GUI.ParseXaml(@"
                <Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Grid.Background>
                        <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
                            <GradientStop Offset=""0"" Color=""#FF123F61""/>
                            <GradientStop Offset=""0.6"" Color=""#FF0E4B79""/>
                            <GradientStop Offset=""0.7"" Color=""#FF106097""/>
                        </LinearGradientBrush>
                    </Grid.Background>
                    <Viewbox>
                        <StackPanel Margin=""50"">
                            <Button Content=""Hello World!"" Margin=""0,30,0,0""/>
                            <Rectangle Height=""5"" Margin=""-10,20,-10,0"">
                                <Rectangle.Fill>
                                    <RadialGradientBrush>
                                        <GradientStop Offset=""0"" Color=""#40000000""/>
                                        <GradientStop Offset=""1"" Color=""#00000000""/>
                                    </RadialGradientBrush>
                                </Rectangle.Fill>
                            </Rectangle>
                        </StackPanel>
                    </Viewbox>
                </Grid>");
            
            _guiView = Noesis.GUI.CreateView(xaml);
            RefreshGUISize();
            _guiView.Renderer.Init(guiDevice);
            _guiView.SetFlags(Noesis.RenderFlags.PPAA | Noesis.RenderFlags.LCD);
            
            // ToDo: register resize event
        }

        private void RefreshGUISize()
        {
            var viewport = GraphicsDevice.Viewport;
            _guiView.SetSize((ushort)viewport.Width, (ushort)viewport.Height);
        }
        
        protected override void UnloadContent()
        {
            if (_guiView != null)
            {
                // ToDo: unsubscribe events (like resize)...
                
                _guiView.Renderer.Shutdown();
                _guiView = null;
            }

            // unload content here if necessary ...
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Keyboard.GetState().IsKeyDown(Keys.Escape)) )
            {
                Exit();
            }
            
            // update your game state here ...
            
            // ToDo: Add mouse events for GUI here ...
            // ToDo: Add keyboard events for GUI here ...
            
            _guiView.Update(gameTime.TotalGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            {
                using var renderState = new D3X11RenderState(GraphicsDevice);
                _guiView.Renderer.UpdateRenderTree();
                _guiView.Renderer.RenderOffscreen();
            }

            // draw your game state here ...
            
            _guiView.Renderer.Render();
            
            base.Draw(gameTime);
        }

    }
}