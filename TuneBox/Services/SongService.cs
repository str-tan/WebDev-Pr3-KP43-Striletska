using AutoMapper;
using TuneBox.DTOs;
using TuneBox.Models;
using TuneBox.Repositories.Interfaces;

namespace TuneBox.Services
{
    public class SongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IMapper _mapper;

        public SongService(ISongRepository songRepository, IArtistRepository artistRepository, IMapper mapper)
        {
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _mapper = mapper;
        }

        public IEnumerable<SongResponseDto> GetAllSongs()
        {
            var songs = _songRepository.GetAll();
            return _mapper.Map<IEnumerable<SongResponseDto>>(songs);
        }

        public SongResponseDto? GetSongById(int id)
        {
            var song = _songRepository.GetById(id);
            return song == null ? null : _mapper.Map<SongResponseDto>(song);
        }

        public (SongResponseDto? SongDto, string? ErrorMessage) CreateSong(SongCreateUpdateDto newSongDto)
        {
            if (!_artistRepository.Exists(newSongDto.ArtistId))
            {
                return (null, $"Artist with ID {newSongDto.ArtistId} does not exist.");
            }

            var song = _mapper.Map<Song>(newSongDto);
            _songRepository.Add(song);
            return (_mapper.Map<SongResponseDto>(song), null);
        }

        public (bool IsSuccess, bool IsNotFound, string? ErrorMessage) UpdateSong(int id, SongCreateUpdateDto updatedSongDto)
        {
            var existingSong = _songRepository.GetById(id);
            if (existingSong == null) return (false, true, null);

            if (!_artistRepository.Exists(updatedSongDto.ArtistId))
            {
                return (false, false, $"Artist with ID {updatedSongDto.ArtistId} does not exist.");
            }

            _mapper.Map(updatedSongDto, existingSong);
            _songRepository.Update(existingSong);
            return (true, false, null);
        }

        public bool DeleteSong(int id)
        {
            var songToRemove = _songRepository.GetById(id);
            if (songToRemove == null) return false;

            _songRepository.Delete(songToRemove);
            return true;
        }

        public void DeleteAllSongs()
        {
            _songRepository.DeleteAll();
        }
    }
}