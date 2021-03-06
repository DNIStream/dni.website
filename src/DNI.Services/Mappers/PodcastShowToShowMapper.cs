﻿using DNI.Services.Podcast;
using DNI.Services.Shared.Mapping;
using DNI.Services.Show;

namespace DNI.Services.Mappers {
    /// <summary>
    ///     Maps a service layer <see cref="PodcastShow" /> to an application layer <see cref="Show" />
    /// </summary>
    public class PodcastShowToShowMapper : IMapper<PodcastShow, Show.Show> {
        public Show.Show Map(PodcastShow show) {
            return new Show.Show {
                Title = show.Title,
                Summary = show.Summary,
                AudioUrl = show.AudioFile?.Url,
                PublishedTime = show.PublishedTime,
                Version = show.Version,
                ImageUrl = show.HeaderImage,
                ShowNotes = show.Content,
                ShowNotesHtml = show.ContentHtml,
                PodcastPageUrl = show.PageUrl,
                Duration = show.AudioFile?.Duration,
                Slug = show.Slug,
                Keywords = show.Keywords,
                DurationInSeconds = show.DurationInSeconds
            };
        }
    }
}