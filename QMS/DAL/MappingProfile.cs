using AutoMapper;
using QMS.DTO;

namespace QMS.DAL;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map FrontDeskTerminal to TerminalInfoView
        CreateMap<FrontDeskTerminal, TerminalInfoView>()
            .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.DeviceName));
        // Map Ticket to TaskView
        CreateMap<Ticket, TicketView>()
            .ForMember(dest => dest.FrontDeskTerminalInfo, opt => opt.MapFrom(src => src.FrontDeskTerminal));
    }
}
