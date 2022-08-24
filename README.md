# EcgMap
A map to create info markers for ecogood members.
This WebApp is based on the OpenSource [Leaflet.js library](https://leafletjs.com/) for creating markers on open street map.

## Usage
Copy the content of the ```wwwroot.zip``` of the latest release to your webserver domain folder. You can find a demonstration of this app [here](https://maximilian-segelken.de).

The app can be configured to show markers on a map, that can be zoomed in, listed, searched and sorted. The configuration of the markers works by passing the url of a json file into the app, which will then create you an iframe code, that you can embed into your (e.g. wordpress-) site.

![image](https://user-images.githubusercontent.com/93731009/186474932-1709c623-7287-4974-9f0b-e36404ae563d.png)

![image](https://user-images.githubusercontent.com/93731009/186477285-e6f671b7-1c0b-43b3-8680-df4082ce6402.png)

This json file needs to have to be structured as follows:
```json
{
  "center": {
    "lon": 9.9617,
    "lat": 53.5596,
    "zoom": 10
  },
  "locations": [
    {
      "title": "GWÖ Hamburg",
      "description": "Büro der GWÖ Hamburg",
      "kind": "text",
      "type": "main",
      "imageUrl": "",
      "imageWidth": 0,
      "link": "",
      "email": "",
      "lon": 9.9617,
      "lat": 53.5596
    },
    {
      "title": "Ecg Company",
      "description": "Nice Description for the company",
      "kind": "image",
      "type": "company",
      "imageUrl": "https://www.ecogood.org/wp-content/uploads/2020/04/ecogood_logo_250x65@2x.png",
      "imageWidth": 100,
      "link": "https://www.ecogood.org/",
      "email": "",
      "lon": 9.96136,
      "lat": 53.53613
    }
  ]
}
```
### center
In the center area you will need to find where the map centers on startup and with which zoom-level.

### locations
The locations area is a list of markers, each with certain properties:
#### title
The title to be shown in the left list
#### description
The description to be shown in the left list
#### kind 
the kind can be 'text' or 'image'. On 'image' the given imageUrl is shown in the left list, on text the image url will be ignored
#### type
The type can be 'main' or 'company'. On 'main' a blue marker is shown on the map. On 'company' an ecg-marker is shown.
#### imageUrl
The image to be shown on the left list. Make sure, to use images on an ssl server (starting with 'https://...')
