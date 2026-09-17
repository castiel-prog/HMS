using AutoMapper;
using HMS.Application.DTOs;
using HMS.Domain.Entities;
using static HMS.Application.DTOs.IAuthDtos;
using static HMS.Application.DTOs.ManagerDtos;
// No 'using static' needed now that DTOs are top-level in HMS.Application.DTOs


namespace HMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Hotel Mappings
            CreateMap<Hotel, HotelResponseDto>();
            CreateMap<CreateHotelDto, Hotel>();
            CreateMap<UpdateHotelDto, Hotel>();

            // Room Mappings
            CreateMap<Room, RoomResponseDto>();
            CreateMap<CreateRoomDto, Room>();
            CreateMap<UpdateRoomDto, Room>();

            // Guest Mappings
            CreateMap<Guest, GuestResponseDto>();
            CreateMap<CreateGuestDto, Guest>();
            CreateMap<UpdateGuestDto, Guest>();

            //Manager mapping
            CreateMap<Manager, ManagerResponseDto>();
            CreateMap<CreateManagerDto, Manager>();
            CreateMap<UpdateManagerDto, Manager>();

            // Reservation Mappings
            CreateMap<Reservation, ReservationResponseDto>();
            CreateMap<CreateReservationDto, Reservation>();
            CreateMap<UpdateReservationDto, Reservation>();


            // User Mappings
            CreateMap<User, UserResponseDto>();
        }
    }
}
