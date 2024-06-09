using System;
using NXOpen.Preferences;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Disposable
{
    public class ResetShadeRendering : IDisposable
    {
        private readonly ShadingRenderingStyleOption _original;

        public ResetShadeRendering()
        {
            var preferencesBuilder1 = __work_part_.SettingsManager.CreatePreferencesBuilder();

            using (new Destroyer(preferencesBuilder1))
            {
                _original = preferencesBuilder1.ViewStyle.ViewStyleShading.RenderingStyle;
            }
        }

        public void Dispose()
        {
            var preferencesBuilder1 = __work_part_.SettingsManager.CreatePreferencesBuilder();

            using (new Destroyer(preferencesBuilder1))
            {
                preferencesBuilder1.ViewStyle.ViewStyleShading.RenderingStyle = _original;

                preferencesBuilder1.Commit();
            }
        }
    }
}