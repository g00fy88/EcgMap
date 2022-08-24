using FisSst.BlazorMaps;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Text;
using Yaapii.JSON;

namespace EcgMap.Shared
{
    public partial class LocationsBar : ComponentBase
    {
        [Inject]
        public IJSRuntime JS { get; set; }
        [Parameter]
        public IEnumerable<Location> Locations 
        {
            set
            {
                this.allLocations = value;
                this.Filtered = Filter();
                this.JS.InvokeVoidAsync("scrollToTop");
            }
        }
        [Parameter]
        public Map Map { get; set; }

        private string searchString = "";
        private IEnumerable<Location> allLocations = new List<Location>();

        public IEnumerable<Location> Filtered { get; set; } = new List<Location>();

        public LocationsBar()
        { }

        public async Task ToggleLoc(Location loc)
        {
            await loc.Marker.TogglePopup();
            await this.Map.SetView(new LatLng(loc.Latitude, loc.Longitude));
        }

        public void Search(string value)
        {
            this.searchString = value;
            this.Filtered = Filter();
        }

        private IEnumerable<Location> Filter()
        {
            var result = this.allLocations;
            if(this.searchString != "")
            {
                result =
                    new Filtered<Location>(loc =>
                        loc.Title.Contains(this.searchString, StringComparison.InvariantCultureIgnoreCase) ||
                        loc.Description.Contains(this.searchString, StringComparison.InvariantCultureIgnoreCase),
                        this.allLocations
                    );
            }
            return result;
        }
    }

    public sealed class Location
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Kind { get; set; }
        public string LocationType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int ImageWidth { get; set; }
        public string Link { get; set; }
        public string Email { get; set; }
        public Marker Marker { get; set; }
    }

    public sealed class JsonLocations : ListEnvelope<Location>
    {
        public JsonLocations(IJSON json) : base(() =>
            new Yaapii.Atoms.List.Mapped<IJSON, Location>((node) =>
                new Location()
                {
                    Id = Guid.NewGuid().ToString(),
                    Latitude = new DoubleOf(node.Value("lat")).Value(),
                    Longitude = new DoubleOf(node.Value("lon")).Value(),
                    Kind = node.Value("kind"),
                    LocationType = node.Value("type"),
                    Title = node.Value("title"),
                    Description = node.Value("description"),
                    ImageUrl = node.Value("imageUrl"),
                    ImageWidth = new IntOf(node.Value("imageWidth")).Value(),
                    Link = node.Value("link"),
                    Email = node.Value("email")
                },
                json.Nodes("locations[*]")
            ),
            false
        )
        { }
    }
}
