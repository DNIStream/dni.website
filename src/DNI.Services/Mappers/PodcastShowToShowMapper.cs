using System;

using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.ShowList;

namespace DNI.Services.Mappers {
    /// <summary>
    ///     Maps a service layer <see cref="PodcastShow" /> to an application layer <see cref="Show" />
    /// </summary>
    public class PodcastShowToShowMapper : IMapper<PodcastShow, Show> {
        public Show Map(PodcastShow show) {
            // TODO: Write tests
            return new Show {
                Title = show.Title,
                Summary = show.Summary,
                AudioUrl = show.AudioFile?.Url,
                PublishedTime = show.DatePublished,
                Version = show.Version,
                ImageUrl = show.HeaderImage,
                ShowNotes = show.Content,
                ShowNotesHtml = show.ContentHtml,
                PodcastPageUrl = show.PageUrl,
                Duration = show.AudioFile?.Duration,
                Slug = show.Slug,
                Keywords = show.Keywords,
                DurationInSeconds = TimeSpan.TryParse(show.AudioFile?.Duration, out var durationInSeconds)
                    ? (long?) durationInSeconds.TotalSeconds : null
            };
        }
    }
}