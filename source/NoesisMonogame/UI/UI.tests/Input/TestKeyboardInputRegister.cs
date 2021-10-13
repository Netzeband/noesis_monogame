using System;
using System.Dynamic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NSubstitute;
using NUnit.Framework;


namespace UI.Input
{
    public class TestKeyboardInputRegister
    {
        public enum TestStates
        {
            State1,
            State2,
        }


        private KeyboardInputRegister<TestStates> CreateInstance()
        {
            return new KeyboardInputRegister<TestStates>();
        }

        
        [Test]
        public void TestCreate()
        {
            var register = CreateInstance();
            
            Assert.IsInstanceOf<KeyboardInputRegister<TestStates>>(register);
            Assert.IsInstanceOf<IKeyboardInputRegister<TestStates>>(register);
        }

        
        [Test]
        public void TestRegisterKey()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;
            
            register.RegisterKey(Keys.Escape, TestStates.State1, (_, _, _) => true );
            register.RegisterKey(Keys.Enter, TestStates.State2, (_, _, _) => true );
        }
        
        
        [Test]
        public void TestProcessKeysReturnsAllKeys()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);
            
            Assert.AreEqual(keys, unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysCallsHandler()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(new[]{Keys.Escape}, unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysDoNoProcessKeyWhenHandlerReturnsFalse()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(false);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(keys, unhandledKeys);
        }

        [Test]
        public void TestProcessKeysCallsHandlerOnKeyUp()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(new[]{Keys.Escape}, unhandledKeys);
            
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(Array.Empty<Keys>(), TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(Array.Empty<Keys>(), unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysDontCallHandlerOnKeyUpWhenKeyWasUnprocessed()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(false);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(keys, unhandledKeys);
            
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(Array.Empty<Keys>(), TestStates.State1, gameTime);

            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(Array.Empty<Keys>(), unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysHandleOnlyKeyDown()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, register.AtKeyDown(enterHandler));
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(Array.Empty<Keys>(), TestStates.State1, gameTime);

            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(Array.Empty<Keys>(), unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysHandleOnlyKeyUp()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, register.AtKeyUp(enterHandler));
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(Array.Empty<Keys>(), TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(Array.Empty<Keys>(), unhandledKeys);
        }

    }
}