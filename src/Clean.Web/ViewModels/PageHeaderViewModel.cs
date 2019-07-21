using Umbraco.Core.Models.PublishedContent;

namespace Clean.Web.ViewModels
{
    public class PageHeaderViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IPublishedContent BackgroundImage { get; set; }
        public bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);
        public bool HasBackgroundImage => BackgroundImage != null;

        public PageHeaderViewModel(string name, string title, string subtitle, IPublishedContent backgroundImage)
        {
            Name = name;
            Title = title;
            Subtitle = subtitle;
            BackgroundImage = backgroundImage;
        }
    }
}