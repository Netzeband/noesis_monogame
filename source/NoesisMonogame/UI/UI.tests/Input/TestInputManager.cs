using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace UI.Input
{
    public class TestInputManager
    {
        IMouseInputHandler _mouseInputHandler;
        IMouseStateReader _mouseStateReader;
        IKeyboardInputHandler _keyboardInputHandler;
        IKeyboardStateReader _keyboardStateReader;
        
        [SetUp]
        public void SetUp()
        {
            _mouseInputHandler = Substitute.For<IMouseInputHandler>();
            _mouseStateReader = Substitute.For<IMouseStateReader>();
            _keyboardInputHandler = Substitute.For<IKeyboardInputHandler>();
            _keyboardStateReader = Substitute.For<IKeyboardStateReader>();
        }
        
        private InputManager CreateInstance()
        {
            return new InputManager(_mouseStateReader, _keyboardStateReader, _mouseInputHandler, _keyboardInputHandler);
        }
        
        [Test]
        public void TestCreate()
        {
            var manager = CreateInstance();
            
            Assert.IsInstanceOf<IInputManager>(manager);
            Assert.IsInstanceOf<InputManager>(manager);
        }

        [Test]
        public void TestProcessingOfMouseMovement()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.ProcessMouseMove(Arg.Is(mouseState.X), Arg.Is(mouseState.Y));
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
        }
        
        [Test]
        public void TestProcessingOfMouseWheel()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.ProcessMouseWheel(Arg.Is(mouseState.ScrollWheelValue));
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
        }
        
        
        [Test]
        public void TestProcessingOfMouseButtonsNoButtonsPressed()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
            _mouseInputHandler.DidNotReceive().ProcessButtonPressed(Arg.Any<MouseButtons>());
        }

        
        [Test]
        public void TestProcessingOfMouseButtonsLeftButtonPressed()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Pressed,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Left);
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
        }

        
        [Test]
        public void TestProcessingOfMouseButtonsRightButtonPressed()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Pressed,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Right);
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
        }

        [Test]
        public void TestProcessingOfMouseButtonsMiddleButtonPressed()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var mouseState = new MouseState(
                42,
                32,
                27,
                ButtonState.Released,
                ButtonState.Pressed,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released);
            
            _mouseStateReader.GetState().Returns(mouseState);
            
            manager.Update(gameTime);
            
            Received.InOrder(() =>
            {
                _mouseInputHandler.PrepareProcessing();
                _mouseInputHandler.ProcessButtonPressed(MouseButtons.Middle);
                _mouseInputHandler.Update(Arg.Is(gameTime));
            });
        }

        [Test]
        public void TestProcessingOfKeys()
        {
            var manager = CreateInstance();

            var gameTime = new GameTime();
            var keys = new [] { Keys.Enter, Keys.Escape };
            
            _keyboardStateReader.GetState().Returns(keys);
            
            manager.Update(gameTime);

            _keyboardInputHandler.Received().ProcessKeys(Arg.Is<Keys[]>(k => keys.SequenceEqual(k)), Arg.Is(gameTime));
        }

    }
}