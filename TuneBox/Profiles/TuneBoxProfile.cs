using AutoMapper;
using TuneBox.DTOs;
using TuneBox.Models;

namespace TuneBox.Profiles
{
    public class TuneBoxProfile : Profile
    {
        public TuneBoxProfile()
        {
            // Мапінг для Artist
            CreateMap<Artist, ArtistResponseDto>();
            CreateMap<ArtistCreateUpdateDto, Artist>();

            // Мапінг для Song
            CreateMap<Song, SongResponseDto>();
            CreateMap<SongCreateUpdateDto, Song>();
        }
    }
}