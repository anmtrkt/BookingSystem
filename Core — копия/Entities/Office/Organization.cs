namespace BookingSystem.Core.Entities;

public class Organization : BaseEntity
{
    public string Name { get; private set; }
    public ICollection<Office> Officies { get; private set; } = new List<Office>();
#pragma warning disable CS8618
    public Organization() { }
#pragma warning restore CS8618 
    public Organization(string name)
    {
        Id = new Guid();
        Name = name;
    }
    public void AddOffice(Office office)
    {
        if (Officies.Any(o => o.Id == office.Id)) return;
        Officies.Add(office);
    }
    public void RemoveOffice(Office office)
    {
        var officieToRemove = Officies.FirstOrDefault(o => o.Id == office.Id);
        if (officieToRemove != null)
        {
            Officies.Remove(officieToRemove);
        }
    }
    public void UpdateName(string newName)
    {
        Name = newName;
    }
}
