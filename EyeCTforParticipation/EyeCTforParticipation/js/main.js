//Functions
function leadingZeros(number, length) {
	return number.length < length ? leadingZeros("0" + number, length) : number;
}

function nl2br(str) {
	return str.replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, "$1"+ "<br />" +"$2");
}

function popupTabs(self, method) {
	self.elements.content.children(".tabs").children().click(function() {
		var tab = $(this).data("tab");
		$(this).parent().children().removeClass("current").end().end().addClass("current");
		self.elements.content.children(":not(.tabs)").hide().end().children("[data-tab=" + tab + "]").show();
		if(method) {
			method(tab);
		}
	});
	
	self.elements.content.children("[data-tab=" + self.elements.content.children(".tabs").children(".current").data("tab") + "]").show();
}

$(function() {
	//Login
	function loginPopup() {
		var passwordButtons = [
			{
				text: "Inloggen",
				classes: "primary_button",
				value: true
			},
			{
				text: "Wachtwoord vergeten?",
				classes: "button small_button light_button"
			}
		];
		new jPopup({
			title: "<h3>Inloggen</h3>",
			content: "<ul class=\"tabs\">"
						+"<li class=\"current\" data-tab=\"rfid\">RFID</li>"
						+"<li data-tab=\"password\">Wachtwoord</li>"
					+"</ul>"
					+"<article data-tab=\"rfid\">"
						+"<i class=\"material-icons\">&#xE870;</i>"
						+"<p>Hou de pas bij de scanner om in te loggen.</p>"
					+"</article>"
					+"<article data-tab=\"password\">"
						+"<input type=\"text\" name=\"email\" class=\"input\" placeholder=\"E-mailadres\" />"
						+"<input type=\"password\" name=\"password\" class=\"input\" placeholder=\"Password\" />"
					+"</article>",
			closeButton: true,
			classes: "login_popup",
			overrides: {
				open: function() {
					var self = this;
					popupTabs(this, function(tab) {
						if(tab == "password") {
							self.buttons(passwordButtons);
						} else {
							self.buttons([]);
						}
					});
					return jPopup._super(this);
				}
			}
		}).open(function(r) {
			if(r) {
				var data = this.form().serializeObject();
				$.post("/user/login", {
					email: data.email,
					password: data.password
				}, function(data) {
					if(data.Success) {
						switch(data.Role) {
							case 1:
								window.location.href = "/helpseeker/helprequests"
								break;
						}
					}
				});
			}
		});
	}
	
	$(".login").click(function() {
		loginPopup();
	});
	
	//User	
	function userPopup(data) {
		var age = Math.abs(new Date(new Date() - new Date(data.Birthdate)).getFullYear() - 1970);
		return new jPopup({
			title: "<h3>" + data.Name + "</h3>",
			content: "<div class=\"info\">"
					+(data.Birthdate ? "<h4>Leeftijd</h4>" : "")
					+(data.Birthdate ? "<p>" + age + " jaar</p>" : "")
					+(data.Address ?" <h4>Adres</h4>" : "")
					+(data.Address ? "<p>" + data.Address + "</p>" : "")
					+(data.DriversLicense ?" <h4>Rijbewijs</h4>" : "")
					+(data.DriversLicense ? "<p><i class=\"material-icons\">&#xE5CA;</i>Heeft een rijbewijs.</p>" : "")
					+(data.Car ? "<h4>Auto</h4>" : "")
					+(data.Car ? "<p><i class=\"material-icons\">&#xE5CA;</i>Heeft beschikking tot een auto.</p>" : "")
					+"</div>"
					+(data.About ? "<h4>Over</h4>" : "")
					+(data.About ? "<p>" + nl2br(data.About) + "</p>" : ""),
			closeButton: true,
			classes: "user_popup"
		});
	}
	
	//Helprequests
	var helprequestData = {
		id: 3243,
		title: "Example D",
		urgency: 2,
		location: "Eindhoven",
		content: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla erit controversia. Hunc vos beatum; Zenonis est, inquam, hoc Stoici. Tollenda est atque extrahenda radicitus. Duo Reges: constructio interrete.\n\n"
				+"Itaque ab his ordiamur. Nihil enim hoc differt. Minime vero, inquit ille, consentit. Quae cum dixisset, finem ille. Ego vero isti, inquam, permitto. Confecta res esset. Et nemo nimium beatus est\n\n."
				+"Qui convenit? Bonum valitudo: miser morbus. Sit enim idem caecus, debilis.\n\n"
				+"Equidem, sed audistine modo de Carneade? Itaque ab his ordiamur. Tubulo putas dicere? Quid de Pythagora? Itaque fecimus. Quaerimus enim finem bonorum.\n\n"
				+"Dici enim nihil potest verius. Dici enim nihil potest verius. Quaerimus enim finem bonorum.",
		user: "Thomas Gladdines",
		date: new Date(),
		application: {
			id: 5324,
			status: 1
		}
	};
	
	function helprequestPopup(data, context) {
		var applyText = function() {
			return data.application && data.application.status < 3 ? "<i class=\"material-icons\">&#xE15B;</i>Afmelden" : "<i class=\"material-icons\">&#xE145;</i>Aanmelden";
		};
		var applyButton = new jPopup.button({
			text: applyText,
			classes: "primary_button",
			close: false,
			onclick: function() {
				var self = this;
				if(data.application && data.application.status < 3) {
					new jPopup({
						title: "<h3>Afmelden</h3>",
						content: "<p>Weet je zeker dat je wilt afmelden van deze hulpvraag?</p>",
						buttons: [
							{
								text: "Afmelden",
								classes: "primary_button",
								value: true
							},
							{
								text: "Annuleren",
								classes: "button"
							}
						]
					}).open(function(r) {
						if(r) {
							data.application.status = 3;
							applyButton.text(applyText());
							self.title("<h3>" + title() + "</h3>");
							$(context).children("td:first-child").children("h4").html("<h4>" + title() + "</h>");
							if($(context).closest(".applications").length) {
								$(context).hide();
								self.close();
							}
						}
					});
				} else {
					data.application = {
						id: 5324,
						status: 0
					};
					applyButton.text(applyText());
					this.title("<h3>" + title() + "</h3>");
					$(context).children("td:first-child").children("h4").html("<h4>" + title() + "</h3>");
				}
			}
		})
		var title = function() {
			var urgency = "";
			switch(data.urgency) {
				case 1:
					urgency = "<span class=\"urgency_low\">Belangrijk</span>";
					break;
				case 2:
					urgency = "<span class=\"urgency_normal\">Urgent</span>";
					break;
				case 3:
					urgency = "<span class=\"urgency_critical\">Zeer urgent</span>";
					break;
			}
			if(data.application) {
				switch(data.application.status) {
					case 0:
						urgency = "<span class=\"applied\"><i class=\"material-icons\">&#xE145;</i><i>Aangemeld</i></span>";
						break;
					case 1:
						urgency = "<span class=\"interview\"><i class=\"material-icons\">&#xE7FB;</i><i>Kennismaken</i></span>";
						break;
					case 2:
						urgency = "<span class=\"approved\"><i class=\"material-icons\">&#xE5CA;</i><i>Goedgekeurd</i></span>";
						break;
				}
			}
			return urgency + data.title;
		}
		var date = new Date(data.date);
		return new jPopup({
			title: "<h3>" + title() + "</h3>",
			content: "<div class=\"info\">"
						+"<h4>Gebruiker</h4>"
						+"<p>" + data.user + "</p>"
						+"<h4>Datum</h4>"
						+"<p>" + leadingZeros(date.getDate().toString(), 2) + "-" + leadingZeros((date.getMonth() + 1).toString(), 2) + "-"  + date.getFullYear() + "</p>"
						+(data.location ? "<h4>Locatie</h4>" : "")
						+(data.location ? "<p>" + data.location + "</p>" : "")
					+"</div>"
					+"<h4>Inhoud</h4>"
					+"<p>" + nl2br(data.content) + "</p>",
			closeButton: true,
			classes: "helprequest_popup",
			buttons: [applyButton]
		});
	}
	
	function helprequestManagePopup(data, context) {
		data.Applications.sort(function(a, b) {
			return new Date(a.Date) < new Date(b.Date) ? 1 : -1;
		});
		var title = function() {
			var urgency = "";
			switch(data.HelpRequest.Urgency) {
				case 1:
					urgency = "<span class=\"urgency_low\">Belangrijk</span>";
					break;
				case 2:
					urgency = "<span class=\"urgency_normal\">Urgent</span>";
					break;
				case 3:
					urgency = "<span class=\"urgency_critical\">Zeer urgent</span>";
					break;
			}
			return (data.HelpRequest.Closed ? "<span class=\"closed\"><i class=\"material-icons\">&#xE897;</i><i>Gesloten</i></span>" : urgency) + data.HelpRequest.Title;
		}
		var statusButton = function(x) {
			switch(x) {
				case 0:
					return "<i class=\"material-icons\">&#xE7FB;</i>Kennismaken";
				case 1:
					return "<i class=\"material-icons\">&#xE5CA;</i>Goedkeuren";
				case 2:
					return "<i class=\"material-icons\">&#xE5CD;</i>Afmelden";
			}
		};
		var status = function(x) {
			switch(x) {
				case 1:
					return "<span class=\"interview\"><i class=\"material-icons\">&#xE7FB;</i><i>Kennismaken</i></span>";
				case 2:
					return "<span class=\"approved\"><i class=\"material-icons\">&#xE5CA;</i><i>Goedgekeurd</i></span>";
				default:
					return "";
			}
		};
		var applications = "";
		var approved = false;
		for(var x = 0; x < data.Applications.length; x++) {
			if(data.Applications[x].Status == 2) {
				approved = true;
			}
		}
		for(var x = 0; x < data.Applications.length; x++) {
			var date = new Date(data.Applications[x].Date);
			applications += "<tr data-id=\"" + data.Applications[x].Id + "\">"
								+"<td><h4>" + status(data.Applications[x].Status) + data.Applications[x].Volunteer.Name + "</h4></td>"
								+"<td><time>" + leadingZeros(date.getDate().toString(), 2) + "-" + leadingZeros((date.getMonth() + 1).toString(), 2) + "-" + date.getFullYear() + "</time></td>"
								+"<td><button class=\"primary_button\"" + (approved && data.applications[x].status != 2 ? " disabled" : "") + ">" + statusButton(data.Applications[x].Status) + "</button></td>"
							+"</tr>";
		}
		var closeButtonText = function() {
			return data.HelpRequest.Closed ? "<i class=\"material-icons\">&#xE898;</i>Openen" : "<i class=\"material-icons\">&#xE899;</i>Sluiten"
		}
		var closeButton = new jPopup.button({
			text: closeButtonText(),
			classes: "button",
			close: false,
			onclick: function() {
				data.HelpRequest.Closed = !data.HelpRequest.Closed;
				closeButton.text(closeButtonText());
				closeButton._parents[0].title("<h3>" + title() + "</h3>");
				if(context) {
					$(context).children("td:first-child").children("h4").html(title());
				}
				closeButton._parents[0].elements.content.find(".tabs li:first-child").click().next().toggleClass("disabled");
			}
		})
		return new jPopup({
			title: "<h3>" + title() + "</h3>",
			content: "<ul class=\"tabs\">"
						+"<li class=\"current\" data-tab=\"info\">Info</li>"
						+"<li data-tab=\"applications\"" + (data.HelpRequest.Closed ? " class=\"disabled\"" : "") + ">Aanmeldingen</li>"
					+"</ul>"
					+"<article class=\"info\" data-tab=\"info\">"
						+(data.HelpRequest.Address ? "<h4>Locatie</h4>" : "")
						+(data.HelpRequest.Address ? "<p>" + data.HelpRequest.Address + "</p>" : "")
						+"<h4>Inhoud</h4>"
						+"<p>" + nl2br(data.HelpRequest.Content) + "</p>"
					+"</article>"
					+"<article class=\"applications\" data-tab=\"applications\">"
						+"<table>" + applications + "</table>"
					+"</article>",
			closeButton: true,
			classes: "helprequest_manage_popup",
			buttons: [
				{
					text: "<i class=\"material-icons\">&#xE3C9;</i>Aanpassen",
					classes: "primary_button",
					close: false,
					onclick: function() {
						var self = this;
						helprequestEditPopup(data).open(function(r) {
							if(r) {
								var f = this.form();
								data.HelpRequest.Title = f.find(".title").val();
								data.HelpRequest.Urgency = parseInt(0 + f.find(".urgency").val());
								data.HelpRequest.Address = f.find(".location").val();
								data.HelpRequest.Content = f.find(".content").val();
								self.title("<h3>" + title() + "</h3>");
								self.elements.content.children(".info").html((data.HelpRequest.Address ? "<h4>Locatie</h4>" : "")
																			+(data.HelpRequest.Address ? "<p>" + data.HelpRequest.Address + "</p>" : "")
																			+"<h4>Inhoud</h4>"
																			+"<p>" + nl2br(data.HelpRequest.Content) + "</p>");
								if(context) {
									$(context).children("td:first-child").children("h4").html(title());
									$(context).children("td:last-child").html(data.HelpRequest.Address ? data.HelpRequest.Address : "-");
								}
							}
							
						});
					}
				},
				closeButton
			],
			overrides: {
				open: function() {
					var self = this;
					popupTabs(this);
					this.elements.content.children(".applications").find("tr").click(function() {
						for(var x = 0; x < data.Applications.length; x++) {
							if(data.Applications[x].Id == $(this).data("id")) {
								userPopup(data.Applications[x].Volunteer).open();
							}
						}
					});
					this.elements.content.children(".applications").find("button").click(function(e) {
						e.stopPropagation();
						for(var x = 0; x < data.Applications.length; x++) {
							if(data.Applications[x].Id == $(this).parent().parent().data("id")) {
								data.Applications[x].Status = data.Applications[x].Status == 2 ? 0 : data.Applications[x].Status + 1;
								if(data.Applications[x].Status == 0 || data.Applications[x].Status == 2) {
									self.elements.content.children(".applications").find("h4").children("span").remove();
								}
								if(data.Applications[x].Status == 1) {
									$(this).parent().parent().find("h4").html(status(1) + data.Applications[x].Volunteer.Name);
								}
								if(data.Applications[x].Status == 2) {
									for(var y = 0; y < data.Applications.length; y++) {
										if(data.Applications[y].Id != data.Applications[x].Id) {
											data.Applications[y].Status = 0;
										}
									}
									self.elements.content.children(".applications").find("button").html(statusButton(0)).prop("disabled", true);
									$(this).parent().parent().find("h4").html(status(2) + data.Applications[x].Volunteer.Name);
								}
								if(data.Applications[x].Status == 0) {
									self.elements.content.children(".applications").find("button").prop("disabled", false);
								}
								$(this).html(statusButton(data.Applications[x].Status)).prop("disabled", false);
							}
						}
					});
					return jPopup._super(this);;
				}
			}
		});
	}
	
	function helprequestEditPopup(data) {
		var popup = new jPopup({
			title: "<h3>Hulpvraag toevoegen</h3>",
			content: "<input type=\"text\" class=\"input title\" placeholder=\"Titel\" />"
					+"<select class=\"input urgency\" required>"
						+"<option value=\"\" selected disabled>Urgentie</option>"
						+"<option value=\"0\">Geen</option>"
						+"<option value=\"1\">Belangrijk</option>"
						+"<option value=\"2\">Urgent</option>"
						+"<option value=\"3\">Zeer urgent</option>"
					+"</select>"
					+"<input type=\"text\" class=\"input location\" placeholder=\"Locatie\" />"
					+"<textarea class=\"input content\" rows=\"2\" placeholder=\"Inhoud\"></textarea>",
			buttons: [
				{
					text: "Opslaan",
					classes: "primary_button",
					value: true
				},
				{
					text: "Annuleren",
					classes: "button"
				}
			],
			closeButton: true,
			classes: "helprequest_edit_popup",
			overrides: {
				open: function() {
					var s = jPopup._super(this);
					autosize(this.elements.content.find("textarea")).on("autosize:resized", function() {
						s.position("center");
					});
					s.position("center");
					return s;
				}
			}
		});
		if(data) {
			popup.title("<h3>Hulpvraag aanpassen</h3>");
			popup.elements.content.children(".title").val(data.title);
			popup.elements.content.children(".urgency").val(data.urgency);
			popup.elements.content.children(".location").val(data.location);
			popup.elements.content.children(".content").val(data.content);
		}
		return popup;
	}
	
	$("table.search_results").on("click", "tr:not(:first-child)", function() {
		helprequestPopup(helprequestData, this).open();
	});
	
	$("table.helprequests").on("click", "tr:not(:first-child)", function() {
		var self = this;
		$.get("/helprequest/get", {
			id: 8
		}, function(data) {
			if(data.Success) {
				helprequestManagePopup(data, self).open();
			}
		});
	});
	
	$(".add_helprequest").click(function() {
		helprequestEditPopup().open(function(r) {
			if(r) {
				var f = this.form();
				var helprequest = {
					title: f.find(".title").val(),
					urgency: parseInt(0 + f.find(".urgency").val()),
					location: f.find(".location").val(),
					content: f.find(".content").val()
				}
			}
		});
	});
	
	//Footer
	$(".go_to_top").click(function() {
		$("html, body").animate({
			scrollTop: 0
		}, 500);
	});
});