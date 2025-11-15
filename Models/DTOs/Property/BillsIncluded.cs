using Microsoft.EntityFrameworkCore;

namespace RentEZApi.Models.DTOs.Property;

[Owned]
public class BillsIncluded
{
    public bool? Wifi { get; set; }
    public bool? Electricity { get; set; }
    public bool? Water { get; set; }
    public bool? Gas { get; set; }
}
