using AutoMapper;
using TuneBox.DTOs;
using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Services
{
    public class ArtistService
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ISongRepository _songRepository;
        private readonly IMapper _mapper;

        public ArtistService(IArtistRepository artistRepository, ISongRepository songRepository, IMapper mapper)
        {
            _artistRepository = artistRepository;
            _songRepository = songRepository;
            _mapper = mapper;
        }

        public IEnumerable<ArtistResponseDto> GetAllArtists()
        {
            var artists = _artistRepository.GetAll();
            return _mapper.Map<IEnumerable<ArtistResponseDto>>(artists);
        }

        public ArtistResponseDto? GetArtistById(int id)
        {
            var artist = _artistRepository.GetById(id);
            return artist == null ? null : _mapper.Map<ArtistResponseDto>(artist);
        }

        public ArtistResponseDto CreateArtist(ArtistCreateUpdateDto newArtistDto)
        {
            var artist = _mapper.Map<Artist>(newArtistDto);
            _artistRepository.Add(artist);
            return _mapper.Map<ArtistResponseDto>(artist);
        }

        public bool UpdateArtist(int id, ArtistCreateUpdateDto updatedArtistDto)
        {
            var existingArtist = _artistRepository.GetById(id);
            if (existingArtist == null) return false;

            _mapper.Map(updatedArtistDto, existingArtist);
            _artistRepository.Update(existingArtist);
            return true;
        }

        public bool DeleteArtist(int id)
        {
            var artistToRemove = _artistRepository.GetById(id);
            if (artistToRemove == null) return false;

            _artistRepository.Delete(artistToRemove);
            return true;
        }

        public void DeleteAllArtists()
        {
            _artistRepository.DeleteAll();
        }

        public IEnumerable<SongResponseDto>? GetSongsByArtist(int id)
        {
            if (!_artistRepository.Exists(id)) return null;

            var songs = _songRepository.GetByArtistId(id);
            return _mapper.Map<IEnumerable<SongResponseDto>>(songs);
        }
    }
}