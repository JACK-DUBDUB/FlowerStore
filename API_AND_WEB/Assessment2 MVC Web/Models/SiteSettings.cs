namespace Assessment2_MVC_Web.Models
{
    /// <summary>
    /// Centralized store/site branding settings.
    /// Configure in appsettings.json under the "Site" section.
    /// </summary>
    public class SiteSettings
    {
        /// <summary>
        /// Short/URL-friendly name, e.g. "petal lane"
        /// </summary>
        public string Name { get; set; } = "petal lane";

        /// <summary>
        /// Nicely capitalized display name, e.g. "Petal Lane"
        /// </summary>
        public string DisplayName { get; set; } = "Petal Lane";

        /// <summary>
        /// Optional emoji or icon used in branding.
        /// </summary>
        public string Emoji { get; set; } = "🌿";

        /// <summary>
        /// Short tagline shown in hero and marketing areas.
        /// </summary>
        public string Tagline { get; set; } = "Curated flowers from local Perth growers";

        /// <summary>
        /// Public contact email.
        /// </summary>
        public string ContactEmail { get; set; } = "hello@example.com";
    }
}
