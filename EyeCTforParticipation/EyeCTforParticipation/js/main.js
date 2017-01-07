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
				value: true,
				close: false
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
					if(data) {
						window.location.href = "/user/account";
					} else {
						new jPopup({
							title: "<h3>Error</h3>",
							content: "<p>Verkeerd e-mailadres en/of wachtwoord.</p>",
							buttons: [
								{
									text: "Ok",
									classes: "primary_button"
								}
							],
							closeButton: true
						}).open();
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
		var age = Math.abs(new Date(new Date() - new Date(data.birthdate)).getFullYear() - 1970);
		return new jPopup({
			title: "<h3>" + data.name + "</h3>",
			content: "<div class=\"info\">"
					+(data.birthdate ? "<h4>Leeftijd</h4>" : "")
					+(data.birthdate ? "<p>" + age + " jaar</p>" : "")
					+(data.address ?" <h4>Adres</h4>" : "")
					+(data.address ? "<p>" + data.address + "</p>" : "")
					+(data.driversLicense ?" <h4>Rijbewijs</h4>" : "")
					+(data.driversLicense ? "<p><i class=\"material-icons\">&#xE5CA;</i>Heeft een rijbewijs.</p>" : "")
					+(data.car ? "<h4>Auto</h4>" : "")
					+(data.car ? "<p><i class=\"material-icons\">&#xE5CA;</i>Heeft beschikking tot een auto.</p>" : "")
					+"</div>"
					+(data.about ? "<h4>Over</h4>" : "")
					+(data.about ? "<p>" + nl2br(data.about) + "</p>" : ""),
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
			return data.application && (!data.application.status || data.application.status < 3) ? "<i class=\"material-icons\">&#xE15B;</i>Afmelden" : "<i class=\"material-icons\">&#xE145;</i>Aanmelden";
		};
		var applyButton = new jPopup.button({
			text: applyText,
			classes: "primary_button",
			close: false,
			onclick: function() {
				var self = this;
				if(data.application && (!data.application.status || data.application.status < 3)) {
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
							$.post("/helprequest/cancel", {
								applicationId: data.id
							});
							data.application.status = 3;
							applyButton.text(applyText());
							self.title("<h3>" + title() + "</h3>");
							$(context).children("td:first-child").children("h4").html("<h4>" + title() + "</h>");
							var applications = $(context).closest(".applications");
							if(applications.length) {
								$(context).remove();
								self.close();
								if(applications.find("tr").length == 1) {
									$("#content .info").show();
								}
							}
						}
					});
				} else {
					$.post("/helprequest/apply", {
						id: data.id
					}, function() {
						$.get("/helprequest/get", {
							id: data.id
						}, function(application) {
							data.application = application;
							applyButton.text(applyText());
							self.title("<h3>" + title() + "</h3>");
							$(context).children("td:first-child").children("h4").html("<h4>" + title() + "</h3>");
						});
					});
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
					case 1:
						urgency = "<span class=\"interview\"><i class=\"material-icons\">&#xE7FB;</i><i>Kennismaken</i></span>";
						break;
					case 2:
						urgency = "<span class=\"approved\"><i class=\"material-icons\">&#xE5CA;</i><i>Goedgekeurd</i></span>";
						break;
					case 3:
						break;
					default:
						urgency = "<span class=\"applied\"><i class=\"material-icons\">&#xE145;</i><i>Aangemeld</i></span>";
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
						+"<p>" + data.helpSeeker.name + "</p>"
						+"<h4>Datum</h4>"
						+"<p>" + leadingZeros(date.getDate().toString(), 2) + "-" + leadingZeros((date.getMonth() + 1).toString(), 2) + "-"  + date.getFullYear() + "</p>"
						+(data.address ? "<h4>Locatie</h4>" : "")
						+(data.address ? "<p>" + data.address + "</p>" : "")
					+"</div>"
					+"<h4>Inhoud</h4>"
					+"<p>" + nl2br(data.content) + "</p>",
			closeButton: true,
			classes: "helprequest_popup",
			buttons: [applyButton]
		});
	}
	
	function helpRequestManagePopup(data, context) {
		data.applications.sort(function(a, b) {
			return new Date(a.date) < new Date(b.date) ? 1 : -1;
		});
		var title = function() {
			var urgency = "";
			switch(data.helpRequest.urgency) {
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
			return (data.helpRequest.closed ? "<span class=\"closed\"><i class=\"material-icons\">&#xE897;</i><i>Gesloten</i></span>" : urgency) + data.helpRequest.title;
		}
		var statusButton = function(x) {
			switch(x) {
				case 1:
					return "<i class=\"material-icons\">&#xE5CA;</i>Goedkeuren";
				case 2:
					return "<i class=\"material-icons\">&#xE5CD;</i>Afmelden";
				default:
					return "<i class=\"material-icons\">&#xE7FB;</i>Kennismaken";
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
		for(var x = 0; x < data.applications.length; x++) {
			if(data.applications[x].status == 2) {
				approved = true;
			}
		}
		for(var x = 0; x < data.applications.length; x++) {
			var date = new Date(data.applications[x].date);
			applications += "<tr data-id=\"" + data.applications[x].id + "\">"
								+"<td><h4>" + status(data.applications[x].status) + data.applications[x].volunteer.name + "</h4></td>"
								+"<td><time>" + leadingZeros(date.getDate().toString(), 2) + "-" + leadingZeros((date.getMonth() + 1).toString(), 2) + "-" + date.getFullYear() + "</time></td>"
								+"<td><button class=\"primary_button\"" + (approved && data.applications[x].status != 2 ? " disabled" : "") + ">" + statusButton(data.applications[x].status) + "</button></td>"
							+"</tr>";
		}
		var closeButtonText = function() {
			return data.helpRequest.closed ? "<i class=\"material-icons\">&#xE898;</i>Openen" : "<i class=\"material-icons\">&#xE899;</i>Sluiten"
		}
		var closeButton = new jPopup.button({
			text: closeButtonText(),
			classes: "button",
			close: false,
			onclick: function() {
				data.helpRequest.closed = !data.helpRequest.closed;
				closeButton.text(closeButtonText());
				closeButton._parents[0].title("<h3>" + title() + "</h3>");
				if(context) {
					$(context).children("td:first-child").children("h4").html(title());
				}
				closeButton._parents[0].elements.content.find(".tabs li:first-child").click().next().toggleClass("disabled");
				if(data.helpRequest.closed) {
					$.post("/helprequest/close", {
						id: data.helpRequest.id
					});
				} else {
					$.post("/helprequest/open", {
						id: data.helpRequest.id
					});
				}
			}
		})
		return new jPopup({
			title: "<h3>" + title() + "</h3>",
			content: "<ul class=\"tabs\">"
						+"<li class=\"current\" data-tab=\"info\">Info</li>"
						+(data.applications.length > 0 ? "<li data-tab=\"applications\"" + (data.helpRequest.closed ? " class=\"disabled\"" : "") + "><b>" + data.applications.length + "</b>Aanmeldingen</li>" : "")
					+"</ul>"
					+"<article class=\"info\" data-tab=\"info\">"
						+(data.helpRequest.address ? "<h4>Locatie</h4>" : "")
						+(data.helpRequest.address ? "<p>" + data.helpRequest.address + "</p>" : "")
						+"<h4>Inhoud</h4>"
						+"<p>" + nl2br(data.helpRequest.content) + "</p>"
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
						helpRequestEditPopup(data.helpRequest).open(function(r) {
							if(r) {
								var f = this.form().serializeObject();
								f.id = data.helpRequest.id;
								f.urgency = f.urgency ? f.urgency : 0;
								$.post("/helprequest/save",	f, function() {
									$.get("/helprequest/get", {
										id: data.helpRequest.id
									}, function(newData) {
										data = newData;
										self.title("<h3>" + title() + "</h3>");
										self.elements.content.children(".info").html((data.helpRequest.address ? "<h4>Locatie</h4>" : "")
																					+(data.helpRequest.address ? "<p>" + data.helpRequest.address + "</p>" : "")
																					+"<h4>Inhoud</h4>"
																					+"<p>" + nl2br(data.helpRequest.content) + "</p>");
										if(context) {
											$(context).children("td:first-child").children("h4").html(title());
											$(context).children("td:last-child").html(data.helpRequest.address ? data.helpRequest.address : "-");
										}
									});
								});
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
						for(var x = 0; x < data.applications.length; x++) {
							if(data.applications[x].id == $(this).data("id")) {
								userPopup(data.applications[x].volunteer).open();
							}
						}
					});
					this.elements.content.children(".applications").find("button").click(function(e) {
						e.stopPropagation();
						var id = $(this).parent().parent().data("id");
						for(var x = 0; x < data.applications.length; x++) {
							if(data.applications[x].id == id) {
								data.applications[x].status = "status" in data.applications[x] ? data.applications[x].status == 2 ? 0 : data.applications[x].status + 1 : 1;
								switch(data.applications[x].status) {
									case 0:
										$.post("/helprequest/cancel", {
											applicationId: id
										});
										break;
									case 1:
										$.post("/helprequest/interview", {
											applicationId: id
										});
										break;
									case 2:
										$.post("/helprequest/approve", {
											applicationId: id
										});
										break;
								}
								if(data.applications[x].status == 0 || data.applications[x].status == 2) {
									self.elements.content.children(".applications").find("h4").children("span").remove();
								}
								if(data.applications[x].status == 1) {
									$(this).parent().parent().find("h4").html(status(1) + data.applications[x].volunteer.name);
								}
								if(data.applications[x].status == 2) {
									for(var y = 0; y < data.applications.length; y++) {
										if(data.applications[y].id != data.applications[x].id) {
											data.applications[y].status = 0;
										}
									}
									self.elements.content.children(".applications").find("button").html(statusButton(0)).prop("disabled", true);
									$(this).parent().parent().find("h4").html(status(2) + data.applications[x].volunteer.name);
								}
								if(data.applications[x].status == 0) {
									self.elements.content.children(".applications").find("button").prop("disabled", false);
								}
								$(this).html(statusButton(data.applications[x].status)).prop("disabled", false);
							}
						}
					});
					return jPopup._super(this);;
				}
			}
		});
	}
	
	function helpRequestEditPopup(data) {
		var popup = new jPopup({
			title: "<h3>Hulpvraag toevoegen</h3>",
			content: "<input type=\"text\" name=\"title\" class=\"input\" placeholder=\"Titel\" />"
					+"<select name=\"urgency\" class=\"input\" required>"
						+"<option value=\"\" selected disabled>Urgentie</option>"
						+"<option value=\"0\">Geen</option>"
						+"<option value=\"1\">Belangrijk</option>"
						+"<option value=\"2\">Urgent</option>"
						+"<option value=\"3\">Zeer urgent</option>"
					+"</select>"
					+"<input type=\"text\" name=\"address\" class=\"input\" placeholder=\"Locatie\" />"
					+"<textarea name=\"content\" class=\"input\" rows=\"4\" placeholder=\"Inhoud\"></textarea>",
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
			popup.elements.content.children("[name=title]").val(data.title);
			popup.elements.content.children("[name=urgency]").val(data.urgency ? data.urgency : 0);
			popup.elements.content.children("[name=address]").val(data.address);
			popup.elements.content.children("[name=content]").val(data.content);
		}
		return popup;
	}
	
	$("table.search_results").on("click", "tr:not(:first-child)", function() {
		var self = this;
		$.get("/helprequest/get", {
			id: $(this).data("id")
		}, function(data) {
			if(data) {
				helprequestPopup(data, self).open();
			}
		});
	});
	
	$("table.helprequests").on("click", "tr:not(:first-child)", function() {
		var self = this;
		$.get("/helprequest/get", {
			id: $(this).data("id")
		}, function(data) {
			if(data) {
				helpRequestManagePopup(data, self).open();
			}
		});
	});
	
	$(".add_helprequest").click(function() {
		helpRequestEditPopup().open(function(r) {
			if(r) {
				$.post("/helprequest/save", this.form().serializeObject(), function() {
					location.reload();
				});
			}
		});
	});
	
	//Account
	function account_form(context, method, reset) {
		var form = $(context).prop("disabled", true).closest("form");
		form.find("input").one("invalid", function(e) {
			e.preventDefault();
		});
		if(!form[0].checkValidity()) {
			var invalid = form.find(":invalid")[0];
			new jPopup({
				title: "<h3>Error</h3>",
				content: "<p>" + invalid.validationMessage + "</p>",
				buttons: [
					{
						text: "Ok",
						classes: "primary_button"
					}
				],
				closeButton: true,
				overlayClose: true,
				overrides: {
					close: function() {
						var s = jPopup._super(this);
						$(context).prop("disabled", false);
						setTimeout(function() {
							$(invalid).addClass("invalid").focus().one("input blur", function(e) {
								$(this).removeClass("invalid");
							});
						}, this.speed());
						return s;
					}
				}
			}).open();
		} else {
			var data = form.serializeObject();
			$.post(form.attr("action"), data, function(success) {
				setTimeout(function() {
					$(context).prop("disabled", false);
				}, 1000);
				if(reset) {
					form[0].reset();
				}
				if(method) {
					method(data, success);
				}
			});
		}
	}
	
	$(".profile_form button").click(function(e) {
		e.preventDefault();
		account_form(this, function(data) {
			$("#header .user p").html(data.name);
			new jPopup({
				title: "<h3>Profiel wijzigen</h3>",
				content: "<p>Profiel is gewijzigd.</p>",
				buttons: [
					{
						text: "Ok",
						classes: "primary_button"
					}
				],
				closeButton: true
			}).open();
		});
	});
	$(window).load(function() {
		
	autosize($(".profile_form textarea"));
	//autosize.update($(".profile_form textarea"));
	});
	
	$(".email_form button").click(function(e) {
		e.preventDefault();
		account_form(this, function(data, success) {
			var message =  "<p>Er is een e-mail verstuurt om de e-mailadres wijziging te bevestigen.</p>";
			switch(success) {
				case "InvalidPassword":
					message = "<p>Wachtwoord is onjuist.</p>";
					break;
				case "InvalidEmail":
					message = "<p>E-mailadres is onjuist.</p>";
					break;
			}
			new jPopup({
				title: "<h3>E-mailadres wijzigen</h3>",
				content: message,
				buttons: [
					{
						text: "Ok",
						classes: "primary_button"
					}
				],
				closeButton: true
			}).open();
		}, true);
	});
	
	$(".password_form button").click(function(e) {
		e.preventDefault();
		account_form(this, function(data, success) {
			new jPopup({
				title: "<h3>Wachtwoord wijzigen</h3>",
				content: (success ? "<p>Er is een e-mail verstuurt om de wachtwoord wijziging te bevestigen.</p>" : "<p>Wachtwoord is onjuist.</p>"),
				buttons: [
					{
						text: "Ok",
						classes: "primary_button"
					}
				],
				closeButton: true
			}).open();
		}, true);
	})
	
	$(".password_form input[name=newPasswordRepeat]").on("input", function() {
		if($(this).val() != $(".password_form input[name=newPassword]").val()) {
			this.setCustomValidity("Wachtwoorden moeten overeenkomen.");
		} else {
			this.setCustomValidity("");
		}
	});
	
	$(".remove_form button").click(function(e) {
		e.preventDefault();
		account_form(this, function(data) {
		});
	});
	
	//Footer
	$(".go_to_top").click(function() {
		$("html, body").animate({
			scrollTop: 0
		}, 500);
	});
});