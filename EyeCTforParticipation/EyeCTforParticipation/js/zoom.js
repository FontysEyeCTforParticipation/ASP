var originalResponsiveWidth = jPopup.plugins.responsive.vars.width;

function setZoom(zoom) {
	less.modifyVars({"@zoom": zoom / 100});
	$("body").css("zoom", zoom / 100);
	jPopup.plugins.responsive.vars.width = function() {
		return originalResponsiveWidth() / (zoom / 100);
	};
	$(window).trigger("resize");
	$(".zoom_level").html(zoom + "%");
	$(".zoom_in").prop("disabled", zoom == 150);
	$(".zoom_out").prop("disabled", zoom == 100);
}

function updateZoom(delta) {
	zoom += delta;
	$.post("/user/zoom",
	{
		zoom: zoom,
		userId: userId
	});
	setZoom(zoom);
}

setZoom(zoom);


$(".zoom_in").click(function() {
	updateZoom(5);
});

$(".zoom_out").click(function() {
	updateZoom(-5);
});