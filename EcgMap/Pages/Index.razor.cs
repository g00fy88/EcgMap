using EcgMap.Shared;
using FisSst.BlazorMaps;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Newtonsoft.Json.Linq;
using System.Net;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;
using Yaapii.JSON;

namespace EcgMap.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private NavigationManager NavManager { get; set; }
        [Inject]
        private HttpClient Client { get; set; }
        [Inject]
        private IMarkerFactory MarkerFactory { get; init; }
        [Inject]
        private IIconFactory IconFactory { get; init; }

        public bool ShowOverlay { get; set; } = false;

        private Map mapRef;
        private MapOptions mapOptions = new MapOptions()
        {
            DivId = "mapId",
            Center = new LatLng(43.5596, 4.9617),
            Zoom = 8,
            UrlTileLayer = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
            SubOptions = new MapSubOptions()
            {
                Attribution = "&copy; <a lhref='http://www.openstreetmap.org/copyright'>OpenStreetMap</a>",
                TileSize = 256,
                ZoomOffset = 0,
                MaxZoom = 19
            }
        };
        public IList<Location> Locations = new List<Location>();

        protected override async Task OnInitializedAsync()
        {
            IJSON result = new JSONOf(new ResourceOf("mapLocationsHamburg.json", typeof(Program)));
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("locationsUrl", out var jsonUrls))
            {
                if (jsonUrls.Count > 0)
                {
                    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    query.Add("url", jsonUrls[0]);
                    var response = await this.Client.GetAsync($"php/json.php?{query}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var text = await response.Content.ReadAsStringAsync();
                        this.Locations = new JsonLocations(new JSONOf(text));
                        await Task.Run(async () =>
                        {
                            while (this.mapRef == null)
                            {
                                await Task.Delay(100);
                            }
                        });
                        if (await this.mapRef.IsInitialized())
                        {
                            await this.mapRef.SetView(
                                new LatLng(
                                    new DoubleOf(result.Value("center.lat")).Value(),
                                    new DoubleOf(result.Value("center.lon")).Value()
                                )
                            );
                            await this.mapRef.SetZoom(new IntOf(result.Value("center.zoom")).Value());
                            foreach (var loc in this.Locations)
                            {
                                var marker =
                                    await new FallbackMap<Task<Marker>>(
                                        new MapOf<Task<Marker>>(
                                            new KvpOf<Task<Marker>>("main", () => MainMarker(loc)),
                                            new KvpOf<Task<Marker>>("company", () => CompanyMarker(loc))
                                        ),
                                        (key) => this.MarkerFactory.CreateAndAddToMap(new LatLng(loc.Latitude, loc.Longitude), this.mapRef)
                                    )[loc.LocationType];
                                await marker.BindPopup(loc.Title);
                                loc.Marker = marker;
                            }
                        }

                        Task.Run(() => MapWatch());
                    }
                }
            }
            else
            {
                ShowOverlay = true;
                StateHasChanged();
            }

        }

        public string UrlBase()
        {
            return this.NavManager.BaseUri;
        }

        private async Task MapWatch()
        {
            if (await this.mapRef.IsInitialized())
            {
                var lastCenter = await this.mapRef.GetCenter();
                while (await this.mapRef.IsInitialized())
                {
                    var center = await this.mapRef.GetCenter();
                    if (center.Lat != lastCenter.Lat || center.Lng != lastCenter.Lng)
                    {
                        this.Locations =
                            this.Locations.OrderBy(loc =>
                            {
                                var latDiff = center.Lat - loc.Latitude;
                                var lngDiff = center.Lng - loc.Longitude;
                                return latDiff * latDiff + lngDiff * lngDiff;
                            }).ToList();
                        await this.InvokeAsync(() => StateHasChanged());
                    }
                    await Task.Delay(1000);
                    lastCenter = center;
                }
            }
        }

        private async Task<Marker> MainMarker(Location loc)
        {
            return await this.MarkerFactory.CreateAndAddToMap(new LatLng(loc.Latitude, loc.Longitude), this.mapRef);
        }

        private async Task<Marker> CompanyMarker(Location loc)
        {
            return await this.MarkerFactory.CreateAndAddToMap(
                new LatLng(loc.Latitude, loc.Longitude),
                this.mapRef,
                await Options()
            );
        }

        private async Task<MarkerOptions> Options()
        {
            return
                new MarkerOptions()
                {
                    Opacity = 1,
                    IconRef = await this.IconFactory.Create(IconOptions())
                };
        }

        private IconOptions IconOptions()
        {
            return
                new IconOptions()
                {
                    IconUrl = "images/ecoPointer.png",
                    IconSize = new Point(40, 40),
                    IconAnchor = new Point(20, 40),
                    PopupAnchor = new Point(0, -33)
                };
        }
    }
}
