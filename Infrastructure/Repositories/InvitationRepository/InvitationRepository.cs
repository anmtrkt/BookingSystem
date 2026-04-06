using BookingSystem.Core.Entities.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Repositories.InvitationRepository;

public class InvitationRepository : IInvitationRepository
{
    private readonly BookingSystemDbContext _context;

    public InvitationRepository(BookingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<MeetingInvitation?> GetByIdAsync(Guid id)
    {
        return await _context.MeetingInvitations.FindAsync(id);
    }

    public async Task<MeetingInvitation?> GetByIdAsyncWithInclude(Guid id)
    {
        return await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
            .Include(mi => mi.Invitee)
            .Include(mi => mi.Inviter)
            .FirstOrDefaultAsync(mi => mi.Id == id);
    }

    public async Task<IEnumerable<MeetingInvitation>> GetByMeetingIdAsync(Guid meetingId)
    {
        return await _context.MeetingInvitations
            .Where(mi => mi.MeetingId == meetingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeetingInvitation>> GetByMeetingIdAsyncWithInclude(Guid meetingId)
    {
        return await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
            .Include(mi => mi.Invitee)
            .Include(mi => mi.Inviter)
            .Where(mi => mi.MeetingId == meetingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeetingInvitation>> GetByInviteeIdAsync(Guid inviteeId)
    {
        return await _context.MeetingInvitations
            .Where(mi => mi.InviteeId == inviteeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeetingInvitation>> GetByInviteeIdAsyncWithInclude(Guid inviteeId)
    {
        return await _context.MeetingInvitations
            .Include(mi => mi.Meeting)
            .Include(mi => mi.Invitee)
            .Include(mi => mi.Inviter)
            .Where(mi => mi.InviteeId == inviteeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeetingInvitation>> GetByInviterIdAsync(Guid inviterId)
    {
        return await _context.MeetingInvitations
            .Where(mi => mi.InviterId == inviterId)
            .ToListAsync();
    }

    public async Task AddAsync(MeetingInvitation invitation)
    {
        await _context.MeetingInvitations.AddAsync(invitation);
    }

    public async Task UpdateAsync(MeetingInvitation invitation)
    {
        _context.MeetingInvitations.Update(invitation);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var invitation = await _context.MeetingInvitations.FindAsync(id);
        if (invitation != null)
        {
            _context.MeetingInvitations.Remove(invitation);
        }
        await Task.CompletedTask;
    }
}
