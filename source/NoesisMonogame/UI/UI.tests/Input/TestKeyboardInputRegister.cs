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
        public void TestProcessKeysCallsHandlerOnlyOnceWhenKeyIsGivenTwoTimes()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Enter, Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received(1)(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            Assert.AreEqual(new[]{Keys.Escape}, unhandledKeys);
        }

        
        [Test]
        public void TestProcessKeysCallsHandlerOnlyOnceWhenKeyIsInLastAndCurrentPressedKeys()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler);
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime1 = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime1);

            enterHandler.Received(1)(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime1));
            Assert.AreEqual(new[]{Keys.Escape}, unhandledKeys);
            
            enterHandler.ClearReceivedCalls();
            var gameTime2 = new GameTime(TimeSpan.FromMilliseconds(1010), TimeSpan.FromMilliseconds(10));
            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime2);

            enterHandler.Received(1)(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime2));
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
        public void TestProcessKeysCallsHandlerOnKeyUpAndDoesNotReturnKeyAsUnconsumed()
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
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(false);
            
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

        
        [Test]
        public void TestRegisterDifferentKeysForDifferentState()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;
            
            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            var escapeHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler );
            register.RegisterKey(Keys.Escape, TestStates.State2, escapeHandler );

            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));
            
            register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            escapeHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());

            enterHandler.ClearReceivedCalls();
            escapeHandler.ClearReceivedCalls();
            
            register.ProcessKeys(keys, TestStates.State2, gameTime);

            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            escapeHandler.Received()(Arg.Is(Keys.Escape), Arg.Is(TestStates.State2), Arg.Is(gameTime));
        }

        
        [Test]
        public void TestRegisterDifferentKeysForSameState()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;
            
            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            var escapeHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            
            register.RegisterKey(Keys.Enter, TestStates.State1, enterHandler );
            register.RegisterKey(Keys.Escape, TestStates.State1, escapeHandler );

            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime = new GameTime(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(10));
            
            register.ProcessKeys(keys, TestStates.State1, gameTime);

            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime));
            escapeHandler.Received()(Arg.Is(Keys.Escape), Arg.Is(TestStates.State1), Arg.Is(gameTime));

            enterHandler.ClearReceivedCalls();
            escapeHandler.ClearReceivedCalls();
            
            register.ProcessKeys(keys, TestStates.State2, gameTime);

            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            escapeHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
        }

        
        [Test]
        public void TestProcessKeysWithDelay()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, register.WithRepeatDelay(enterHandler, TimeSpan.FromMilliseconds(100)));
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime1 = new GameTime(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1));
            var gameTime2 = new GameTime(TimeSpan.FromMilliseconds(60), TimeSpan.FromMilliseconds(60));
            var gameTime3 = new GameTime(TimeSpan.FromMilliseconds(120), TimeSpan.FromMilliseconds(60));
            var gameTime4 = new GameTime(TimeSpan.FromMilliseconds(180), TimeSpan.FromMilliseconds(60));
            var gameTime5 = new GameTime(TimeSpan.FromMilliseconds(240), TimeSpan.FromMilliseconds(60));
            var gameTime6 = new GameTime(TimeSpan.FromMilliseconds(350), TimeSpan.FromMilliseconds(110));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime1);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime1));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime2);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime3);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime3));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime4);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime5);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime5));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime6);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime6));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
        }

        
                [Test]
        public void TestProcessKeysWithDelayButWithInterruption()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, register.WithRepeatDelay(enterHandler, TimeSpan.FromMilliseconds(100)));
            
            var keys1 = new [] { Keys.Escape, Keys.Enter };
            var keys2 = new [] { Keys.Escape};
            var gameTime1 = new GameTime(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1));
            var gameTime2 = new GameTime(TimeSpan.FromMilliseconds(60), TimeSpan.FromMilliseconds(60));
            var gameTime3 = new GameTime(TimeSpan.FromMilliseconds(120), TimeSpan.FromMilliseconds(60));
            var gameTime4 = new GameTime(TimeSpan.FromMilliseconds(180), TimeSpan.FromMilliseconds(60));
            var gameTime5 = new GameTime(TimeSpan.FromMilliseconds(240), TimeSpan.FromMilliseconds(60));
            var gameTime6 = new GameTime(TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(60));
            var gameTime7 = new GameTime(TimeSpan.FromMilliseconds(410), TimeSpan.FromMilliseconds(110));

            var unhandledKeys = register.ProcessKeys(keys1, TestStates.State1, gameTime1);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime1));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys1, TestStates.State1, gameTime2);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys2, TestStates.State1, gameTime3);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(keys1, TestStates.State1, gameTime4);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime4));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys1, TestStates.State1, gameTime5);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys1, TestStates.State1, gameTime6);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime6));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys2, TestStates.State1, gameTime7);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
        }

        
        [Test]
        public void TestProcessKeysWithTwoDelays()
        {
            var register = CreateInstance() as IKeyboardInputRegister<TestStates>;

            var enterHandler = Substitute.For<IKeyboardInputRegister<TestStates>.HandlerFunction>();
            enterHandler(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>()).Returns(true);
            
            register.RegisterKey(Keys.Enter, TestStates.State1, register.WithRepeatDelay(
                enterHandler, 
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(50)
                ));
            
            var keys = new [] { Keys.Escape, Keys.Enter };
            var gameTime1 = new GameTime(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1));
            var gameTime2 = new GameTime(TimeSpan.FromMilliseconds(60), TimeSpan.FromMilliseconds(60));
            var gameTime3 = new GameTime(TimeSpan.FromMilliseconds(120), TimeSpan.FromMilliseconds(60));
            var gameTime4 = new GameTime(TimeSpan.FromMilliseconds(180), TimeSpan.FromMilliseconds(60));
            var gameTime5 = new GameTime(TimeSpan.FromMilliseconds(240), TimeSpan.FromMilliseconds(60));
            var gameTime6 = new GameTime(TimeSpan.FromMilliseconds(350), TimeSpan.FromMilliseconds(110));

            var unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime1);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime1));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime2);
            enterHandler.DidNotReceive()(Arg.Any<Keys>(), Arg.Any<TestStates>(), Arg.Any<GameTime>());
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime3);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime3));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
            
            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime4);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime4));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime5);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime5));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();

            unhandledKeys = register.ProcessKeys(keys, TestStates.State1, gameTime6);
            enterHandler.Received()(Arg.Is(Keys.Enter), Arg.Is(TestStates.State1), Arg.Is(gameTime6));
            Assert.AreEqual(new [] { Keys.Escape }, unhandledKeys);
            enterHandler.ClearReceivedCalls();
        }

    }
}