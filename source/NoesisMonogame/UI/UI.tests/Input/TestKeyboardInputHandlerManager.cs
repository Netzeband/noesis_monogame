using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace UI.Input
{
    public class TestKeyboardInputHandlerManager
    {
        private KeyboardInputHandlerManager CreateInstance()
        {
            return new KeyboardInputHandlerManager();
        }

        [Test]
        public void TestCreate()
        {
            var manager = CreateInstance();
            
            Assert.IsInstanceOf<KeyboardInputHandlerManager>(manager);
            Assert.IsInstanceOf<IKeyboardInputHandler>(manager);
        }
        
        [Test]
        public void TestAdd()
        {
            var manager = CreateInstance();

            manager.Add(InputHandlerPriority.High, Substitute.For<IKeyboardInputHandler>());
            manager.Add(InputHandlerPriority.High, Substitute.For<IKeyboardInputHandler>());
            manager.Add(InputHandlerPriority.Normal, Substitute.For<IKeyboardInputHandler>());
            manager.Add(InputHandlerPriority.Normal, Substitute.For<IKeyboardInputHandler>());
            manager.Add(InputHandlerPriority.Low, Substitute.For<IKeyboardInputHandler>());
            manager.Add(InputHandlerPriority.Low, Substitute.For<IKeyboardInputHandler>());
        }
        
        [Test]
        public void TestProcessKeys()
        {
            var manager = CreateInstance();

            var low1 = Substitute.For<IKeyboardInputHandler>();
            var low2 = Substitute.For<IKeyboardInputHandler>();
            var normal1 = Substitute.For<IKeyboardInputHandler>();
            var normal2 = Substitute.For<IKeyboardInputHandler>();
            var high1 = Substitute.For<IKeyboardInputHandler>();
            var high2 = Substitute.For<IKeyboardInputHandler>();
            
            manager.Add(InputHandlerPriority.Low, low1);
            manager.Add(InputHandlerPriority.Low, low2);
            manager.Add(InputHandlerPriority.Normal, normal1);
            manager.Add(InputHandlerPriority.Normal, normal2);
            manager.Add(InputHandlerPriority.High, high1);
            manager.Add(InputHandlerPriority.High, high2);

            var keys = new[]
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G
            };
            var keysAfterHigh1 = new[] { Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G };
            var keysAfterHigh2 = new[] { Keys.C, Keys.D, Keys.E, Keys.F, Keys.G };
            var keysAfterNormal1 = new[] { Keys.D, Keys.E, Keys.F, Keys.G };
            var keysAfterNormal2 = new[] { Keys.E, Keys.F, Keys.G };
            var keysAfterLow1 = new[] { Keys.F, Keys.G };
            var keysAfterLow2 = new[] { Keys.G };
            
            high1.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHigh1);
            high2.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHigh2);
            normal1.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterNormal1);
            normal2.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterNormal2);
            low1.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterLow1);
            low2.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterLow2);

            var time = new GameTime();

            var returnedKeys = manager.ProcessKeys(keys, time);

            high1.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keys)), Arg.Is(time));
            high2.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keysAfterHigh1)), Arg.Is(time));
            normal1.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keysAfterHigh2)), Arg.Is(time));
            normal2.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keysAfterNormal1)), Arg.Is(time));
            low1.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keysAfterNormal2)), Arg.Is(time));
            low2.Received().ProcessKeys(Arg.Is<Keys[]>(k => k.SequenceEqual(keysAfterLow1)), Arg.Is(time));
            
            Assert.AreEqual(keysAfterLow2, returnedKeys);
        }
        
        [Test]
        public void TestRemoveWhenSamePriority()
        {
            var manager = CreateInstance();

            var handler1 = Substitute.For<IKeyboardInputHandler>();
            var handler2 = Substitute.For<IKeyboardInputHandler>();
            
            manager.Add(InputHandlerPriority.Normal, handler1);
            manager.Add(InputHandlerPriority.Normal, handler2);

            var keys = new[]
            {
                Keys.Enter, Keys.Escape, Keys.Left
            };
            var keysAfterHandler1 = new[] { Keys.Enter, Keys.Left };
            var keysAfterHandler2 = new[] { Keys.Left};
            
            handler1.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHandler1);
            handler2.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHandler2);

            var time = new GameTime();
            
            Assert.AreEqual(keysAfterHandler2, manager.ProcessKeys(keys, time));

            manager.Remove(handler2);
            
            Assert.AreEqual(keysAfterHandler1, manager.ProcessKeys(keys, time));
        }

        [Test]
        public void TestRemoveWhenDifferentPriority()
        {
            var manager = CreateInstance();

            var handler1 = Substitute.For<IKeyboardInputHandler>();
            var handler2 = Substitute.For<IKeyboardInputHandler>();
            
            manager.Add(InputHandlerPriority.High, handler1);
            manager.Add(InputHandlerPriority.Normal, handler2);

            var keys = new[]
            {
                Keys.Enter, Keys.Escape, Keys.Left
            };
            var keysAfterHandler1 = new[] { Keys.Enter, Keys.Left };
            var keysAfterHandler2 = new[] { Keys.Left};
            
            handler1.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHandler1);
            handler2.ProcessKeys(Arg.Any<Keys[]>(), Arg.Any<GameTime>()).Returns(keysAfterHandler2);

            var time = new GameTime();
            
            Assert.AreEqual(keysAfterHandler2, manager.ProcessKeys(keys, time));

            manager.Remove(handler2);
            
            Assert.AreEqual(keysAfterHandler1, manager.ProcessKeys(keys, time));
        }

    }
}