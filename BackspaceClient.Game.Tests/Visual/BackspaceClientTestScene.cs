using osu.Framework.Testing;

namespace BackspaceClient.Game.Tests.Visual
{
    public abstract partial class BackspaceClientTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new BackspaceClientTestSceneTestRunner();

        private partial class BackspaceClientTestSceneTestRunner : BackspaceClientGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
