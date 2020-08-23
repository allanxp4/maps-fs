var map = new google.maps.Map(document.getElementById('map'), {
	center: {lat: 0, lng: 0},
	zoom: 14
})

var marker;
var followPlane = true;

function moveMarker(lat, lng, heading) {
	if(!marker){
		marker = new google.maps.Marker({
		  position: {lat: lat, lng: lng},
		  map: map,
		  //icon: "flight-black-18dp.svg"
		});
	}
	marker.setPosition({lat: lat, lng: lng})
	if(followPlane) {
		map.panTo({lat: lat, lng: lng})
	}
}

var socket = new WebSocket(`ws://${window.location.host}/socket`)
socket.onmessage = function (event) {
	var data = JSON.parse(event.data)
	moveMarker(data.latitude, data.longitude, data.heading)
}

var followButton = document.getElementById('follow-plane')
followButton.addEventListener('change', function() {
    followPlane = this.checked
});

