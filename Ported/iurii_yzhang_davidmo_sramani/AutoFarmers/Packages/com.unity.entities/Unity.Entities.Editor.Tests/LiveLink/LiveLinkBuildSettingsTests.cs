using NUnit.Framework;

namespace Unity.Entities.Editor.Tests.LiveLink
{
    [TestFixture]
    class LiveLinkBuildSettingsTests : LiveLinkTestFixture
    {
        [Test]
        public void Should_Set_Inner_Setting_Container_When_Selected_BuildSettings_Changed()
        {
            var (buildSettingsA, guidA) = CreateBuildSettings();
            var (buildSettingsB, guidB) = CreateBuildSettings();

            LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettingsA;
            Assert.That(LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid, Is.EqualTo(guidA));

            LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettingsB;
            Assert.That(LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid, Is.EqualTo(guidB));
        }

        [Test]
        public void Should_Trigger_Event_When_Selected_BuildSettings_Changed()
        {
            var (buildSettingsA, _) = CreateBuildSettings();
            var (buildSettingsB, _) = CreateBuildSettings();

            LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettingsA;
            var eventHasBeenTriggered = 0;
            try
            {
                LiveLinkBuildSettings.CurrentLiveLinkBuildSettingsChanged += OnCurrentLiveLinkBuildSettingsChanged;

                LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettingsB;
                Assert.That(eventHasBeenTriggered, Is.EqualTo(1));
            }
            finally
            {
                LiveLinkBuildSettings.CurrentLiveLinkBuildSettingsChanged -= OnCurrentLiveLinkBuildSettingsChanged;
            }

            void OnCurrentLiveLinkBuildSettingsChanged()
            {
                eventHasBeenTriggered++;
            }
        }

        [Test]
        public void Should_Not_Trigger_Event_When_Selected_BuildSettings_Is_Identical()
        {
            var (buildSettings, _) = CreateBuildSettings();

            LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettings;
            var eventHasBeenTriggered = 0;
            try
            {
                LiveLinkBuildSettings.CurrentLiveLinkBuildSettingsChanged += OnCurrentLiveLinkBuildSettingsChanged;

                LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettings;
                Assert.That(eventHasBeenTriggered, Is.EqualTo(0));
            }
            finally
            {
                LiveLinkBuildSettings.CurrentLiveLinkBuildSettingsChanged -= OnCurrentLiveLinkBuildSettingsChanged;
            }

            void OnCurrentLiveLinkBuildSettingsChanged()
            {
                eventHasBeenTriggered++;
            }
        }

        [Test]
        public void Should_Return_Null_When_No_BuildSettings_Selected()
        {
            var (buildSettings, _) = CreateBuildSettings();
            LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = buildSettings;

            Assert.DoesNotThrow(() => LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = null);

            Assert.That(LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid, Is.EqualTo(string.Empty));
            Assert.That(LiveLinkBuildSettings.CurrentLiveLinkBuildSettings, Is.Null);
        }
    }
}