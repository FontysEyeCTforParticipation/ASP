$(window).on("load", function() {
	//Data
	var chatsData = {
		2134: {
			title: "Example A",
			interview: true,
			user: "John Doe",
			lastMessage: {
				content: "Good!",
				user: "John Doe",
				date: new Date()
			}
		},
		4523: {
			title: "Example A",
			interview: true,
			user: "Jane Doe",
			lastMessage: {
				content: "What about the deadline?",
				user: "Jane Doe",
				date: new Date().setDate(new Date().getDate() - 1)
			}
		},
		6786: {
			title: "Example B",
			lastMessage: {
				content: "Don't worry we're gonna miss the deadline.",
				date: "2016-10-24T12:56:43.511Z"
			}
		},
		1423: {
			title: "Example C",
			lastMessage: {
				content: "???",
				user: "Jane Doe",
				date: "2016-07-05T12:56:43.511Z"
			}
		}
	};
	if(typeof(isVolunteer) != "undefined") {
		chatsData = {
			2134: {
				title: "Example A",
				interview: true,
				lastMessage: {
					content: "What about the deadline?",
					date: new Date()
				}
			},
			6786: {
				title: "Example B",
				lastMessage: {
					content: "Don't worry we're gonna miss the deadline.",
					date: "2016-10-24T12:56:43.511Z"
				}
			},
			1423: {
				title: "Example C",
				lastMessage: {
					content: "???",
					user: "Thomas Gladdines",
					date: "2016-07-05T12:56:43.511Z"
				}
			}
		};
	}
	
	var conversationData = {
		users: {
			4252: "John Doe"
		},
		messages: [
			{
				content: "Yo",
				date: "2016-12-26T12:56:43.511Z",
				user: 4252
			},
			{
				content: "How is it going?",
				date: "2016-12-26T13:04:43.511Z",
				user: 4252
			},
			{
				content: "Good!",
				date: "2016-12-26T13:21:43.511Z"
			},
			{
				content: "What about the deadline?",
				date: "2016-12-27T14:23:43.511Z",
				user: 4252
			},
			{
				content: "Don't worry we're gonna miss the deadline.",
				date: "2016-12-27T15:01:43.511Z"
			},
			{
				content: "???",
				date: "2016-12-27T15:02:43.511Z",
				user: 4252
			}
		]
	};
	
	//Functions	
	function chatsDataToHtml(data) {
		var interviewHtml = "";
		var html = "";
		var now = new Date();
		var dataArray = [];
		for(id in data) {
			var chat = data[id];
			chat.id = id;
			dataArray.push(chat);
		}
		var data = dataArray.slice(0);
		data.sort(function(a, b) {
			return new Date(a.lastMessage.date) < new Date(b.lastMessage.date) ? 1 : -1;
		});
		for(var x = 0; x < data.length; x++) {
			var date = new Date(data[x].lastMessage.date);
			var dateString;
			var currentMonth = date.getMonth() == now.getMonth() && date.getFullYear() == now.getFullYear();
			if(date.getDate() == now.getDate() && currentMonth) {
				dateString = leadingZeros(date.getHours().toString(), 2) + ":" + leadingZeros(date.getMinutes().toString(), 2);
			} else if(date.getDate() == now.getDate() - 1 && currentMonth) {
				dateString = "Gisteren";
			} else {
				dateString = leadingZeros(date.getDate().toString(), 2) + "-" + leadingZeros((date.getMonth() + 1).toString(), 2) + "-" + date.getFullYear().toString().substr(2,2);
			}
			html += "<li data-id=\"" + data[x].id + "\"><h5>" + (data[x].interview ? (data[x].user ? "<span class=\"interview_i\">" + data[x].user + "</span>" : "") + "<span class=\"interview\"><i class=\"material-icons\">&#xE7FB;</i></span>" : "") + data[x].title + "</h5><p>" + (data[x].lastMessage.user ? "<b>" + data[x].lastMessage.user + "</b>: " : "") + data[x].lastMessage.content + "</p><time>" + dateString+ "</time></li>";
		}
		return html;
	}
	
	function conversationToHtml(data) {
		var months = [ "Januari", "Februari", "Maart", "April", "Mei", "Juni", "Juli", "Augustus", "September", "Oktober", "November", "December" ];
		var html = "";
		var lastDate = null
		var lastUser = null;
		for(var x = 0; x < data.messages.length; x++) {
			var date = new Date(data.messages[x].date);
			var user = data.messages[x].user;
			var name = data.users[user];
			if(!lastDate || (lastDate < date && lastDate.getDate() < date.getDate())) {
				html += "<li class=\"date\"><time>" + date.getDate() + " " + months[date.getMonth()] + " " + date.getFullYear() + "</time></li>";
			}
			lastDate = date;
			html += "<li" + (name ? "" : " class=\"me\"") + ">" + (name && lastUser != user ? "<header>" + name + "</header>" : "") + "<time>" + leadingZeros(date.getHours().toString(), 2) + ":" + leadingZeros(date.getMinutes().toString(), 2) + "</time>" + nl2br(data.messages[x].content) + "</li>";
			lastUser = user;
		}
		return html;
	}
	
	//Open chat popups
	var chatPopups = {};
	
	//Add class to body
	$("body").addClass("chat_page");
	
	//Chat overview popup
	var chatsPopup = new jPopup({
		title: "<h2>Chat</h2>",
		content: "<ul></ul>",
		classes: "chats_popup",
		position: "stretchRight",
		overlay: false,
		freeze: false,
		animations: {
			open: {
				stretchRight: function() {
					this.elements.popup.addClass("open");
				}
			},
			close: {
				stretchRight: function() {
					this.elements.popup.removeClass("open");
				}
			}
		},
		plugins: {
			responsive: {
				0: function() {
					this.speed(300);
					this.close();
					this.overlay(true);
					this.closeButton(true);
				},
				1300: function() {
					this.speed(0);
					this.open();
					this.overlay(false);
					this.closeButton(false);
				}
			}
		},
		overrides: {
			open: function() {
				this.elements.content.children("ul").html(chatsDataToHtml(chatsData));
				this.elements.content.children("ul").on("click", "li", function() {
					var id = $(this).data("id");		
					if(id in chatPopups) {
						chatPopups[id]._zIndex();
					} else {
						var count = 0;
						for(x in chatPopups) {
							count++;
						}
						chatPopups[id] = new jPopup({
							title: "<h3>" + (chatsData[id].interview ? (chatsData[id].user ? "<span class=\"interview_i\">" + chatsData[id].user + "</span>" : "") + "<span class=\"interview\"><i class=\"material-icons\">&#xE7FB;</i><i>Kennnismaken</i></span>" : "") + chatsData[id].title + "</h3>",
							content: "<ul class=\"conversation\"></ul>"
									+"<div class=\"reply\">"
										+"<textarea class=\"input\" rows=\"1\" placeholder=\"Schrijf een bericht...\"></textarea>"
										+"<button class=\"primary_button\"><i class=\"material-icons\">&#xE163;</i></button>"
									+"</div>",
							closeButton: true,
							classes: "chat_popup",
							overlay: false,
							freeze: false,
							offset: {
								x: count * 20,
								y: count * -20
							},
							plugins: {
								draggable: true
							},
							overrides: {
								open: function() {
									var s = jPopup._super(this);
									var conversation = this.elements.content.children(".conversation");
									conversation.html(conversationToHtml(conversationData));
									conversation.scrollTop(conversation[0].scrollHeight);
									autosize(this.elements.content.children(".reply").children(".input")).on("autosize:resized", function() {
										var offset = s.offset();
										s.position("center");
										s.offset(offset);
									});
									return s;
								},
								close: function() {
									var self = this;
									this.elements.content.children(".reply").children(".input").off("autosize:resized");
									setTimeout(function() {
										autosize.destroy(self.elements.content.children(".reply").children(".input"));
									}, this.speed());
									delete chatPopups[id];
									return jPopup._super(this);
								}
							}
						}).open();
					}
				});
				return jPopup._super(this);
			},
			close: function() {
				for(id in chatPopups) {
					chatPopups[id].close();
					delete chatPopups[id];
				}
				this.elements.content.children("ul").off("click");
				return jPopup._super(this);
			}
		}
	});
	chatsPopup.open();
	setTimeout(function() {
		//chatsPopup.open();
	}, 1000);
	
	$(".open_chat").click(function() {
		chatsPopup.open();
	});
});