using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

namespace UI.Input
{
    public class TestCombinedMouseInputHandler
    {
        private CombinedMouseInputHandler CreateInstance()
        {
            return new CombinedMouseInputHandler();
        }


        [Test]
        public void TestCreate()
        {
            var handler = CreateInstance();
            
            Assert.IsInstanceOf<CombinedMouseInputHandler>(handler);
            Assert.IsInstanceOf<IMouseInputHandler>(handler);
        }
        
        
        [Test]
        public void TestAdd()
        {
            var handler = CreateInstance();

            handler.Add(InputHandlerPriority.High, Substitute.For<IMouseInputHandler>());
            handler.Add(InputHandlerPriority.High, Substitute.For<IMouseInputHandler>());
            handler.Add(InputHandlerPriority.Normal, Substitute.For<IMouseInputHandler>());
            handler.Add(InputHandlerPriority.Normal, Substitute.For<IMouseInputHandler>());
            handler.Add(InputHandlerPriority.Low, Substitute.For<IMouseInputHandler>());
            handler.Add(InputHandlerPriority.Low, Substitute.For<IMouseInputHandler>());
        }

        [Test]
        public void TestPrepareProcessingIsCalled()
        {
            var handler = CreateInstance();

            var high1 = Substitute.For<IMouseInputHandler>();
            var high2 = Substitute.For<IMouseInputHandler>();
            var normal1 = Substitute.For<IMouseInputHandler>();
            var normal2 = Substitute.For<IMouseInputHandler>();
            var low1 = Substitute.For<IMouseInputHandler>();
            var low2 = Substitute.For<IMouseInputHandler>();

            handler.Add(InputHandlerPriority.High, high1);
            handler.Add(InputHandlerPriority.High, high2);
            handler.Add(InputHandlerPriority.Normal, normal1);
            handler.Add(InputHandlerPriority.Normal, normal2);
            handler.Add(InputHandlerPriority.Low, low1);
            handler.Add(InputHandlerPriority.Low, low2);
            
            handler.PrepareProcessing();
            
            high1.Received().PrepareProcessing();
            high2.Received().PrepareProcessing();
            normal1.Received().PrepareProcessing();
            normal2.Received().PrepareProcessing();
            low1.Received().PrepareProcessing();
            low2.Received().PrepareProcessing();
        }

        
        [Test]
        public void TestPrepareUpdateIsCalled()
        {
            var handler = CreateInstance();

            var high1 = Substitute.For<IMouseInputHandler>();
            var high2 = Substitute.For<IMouseInputHandler>();
            var normal1 = Substitute.For<IMouseInputHandler>();
            var normal2 = Substitute.For<IMouseInputHandler>();
            var low1 = Substitute.For<IMouseInputHandler>();
            var low2 = Substitute.For<IMouseInputHandler>();

            handler.Add(InputHandlerPriority.High, high1);
            handler.Add(InputHandlerPriority.High, high2);
            handler.Add(InputHandlerPriority.Normal, normal1);
            handler.Add(InputHandlerPriority.Normal, normal2);
            handler.Add(InputHandlerPriority.Low, low1);
            handler.Add(InputHandlerPriority.Low, low2);

            var gameTime = new GameTime();
            
            handler.Update(gameTime);
            
            high1.Received().Update(Arg.Is(gameTime));
            high2.Received().Update(Arg.Is(gameTime));
            normal1.Received().Update(Arg.Is(gameTime));
            normal2.Received().Update(Arg.Is(gameTime));
            low1.Received().Update(Arg.Is(gameTime));
            low2.Received().Update(Arg.Is(gameTime));
        }

        
        [Test]
        public void TestPrepareMouseMove()
        {
            var handler = CreateInstance();

            var high1 = Substitute.For<IMouseInputHandler>();
            var high2 = Substitute.For<IMouseInputHandler>();
            var normal1 = Substitute.For<IMouseInputHandler>();
            var normal2 = Substitute.For<IMouseInputHandler>();
            var low1 = Substitute.For<IMouseInputHandler>();
            var low2 = Substitute.For<IMouseInputHandler>();

            handler.Add(InputHandlerPriority.High, high1);
            handler.Add(InputHandlerPriority.High, high2);
            handler.Add(InputHandlerPriority.Normal, normal1);
            handler.Add(InputHandlerPriority.Normal, normal2);
            handler.Add(InputHandlerPriority.Low, low1);
            handler.Add(InputHandlerPriority.Low, low2);
            
            Assert.AreEqual(false, handler.ProcessMouseMove(42, 72));
            
            high1.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            high2.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            normal1.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            normal2.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            low1.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            low2.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            
            high1.ClearReceivedCalls();
            high2.ClearReceivedCalls();
            normal1.ClearReceivedCalls();
            normal2.ClearReceivedCalls();
            low1.ClearReceivedCalls();
            low2.ClearReceivedCalls();
            
            normal2.ProcessMouseMove(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            Assert.AreEqual(true, handler.ProcessMouseMove(42, 72));
            
            high1.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            high2.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            normal1.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            normal2.Received().ProcessMouseMove(Arg.Is(42), Arg.Is(72));
            low1.DidNotReceive().ProcessMouseMove(Arg.Any<int>(), Arg.Any<int>());
            low2.DidNotReceive().ProcessMouseMove(Arg.Any<int>(), Arg.Any<int>());

        }

        
    }
}