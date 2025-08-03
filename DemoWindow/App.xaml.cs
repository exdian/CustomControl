using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace DemoWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 检查 Segoe Fluent Icons 字体是否存在
            bool fontExists = Fonts.SystemFontFamilies.Any(fontFamily =>
            {
                // 检查字体的英文名称
                if (fontFamily.FamilyNames.TryGetValue(XmlLanguage.GetLanguage("en-us"), out string familyName))
                    return familyName.Equals("Segoe Fluent Icons", StringComparison.OrdinalIgnoreCase);
                return false;
            });
            // 如果字体存在则替换资源
            if (fontExists)
                Resources["WindowCaptionFontIcon"] = new FontFamily("Segoe Fluent Icons");
            base.OnStartup(e);
        }
    }

}
