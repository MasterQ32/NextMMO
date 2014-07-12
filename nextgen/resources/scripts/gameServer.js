var app = require('http').createServer(handler)
var io = require('socket.io')(app);
var fs = require('fs');
var url = require('url');

require("./basicLevel.js");

app.listen(80);

var global = require('socket.io-client')('http://localhost:88');
// Add a connect listener
global.on('connect', function(socket) { 
    //console.log('Connected!');
});
global.emit('delta', { });

function prepareLevel(level) {
	function setTile(x, y, height, tex) {
		level.at(x,y).height = height;
		level.at(x,y).textureName = tex;
	}
	for(var y = 0; y <= 5; y++)
	{
		setTile(5, y, -4, "dirt_pave.png");
	}
	setTile(4, 4, -4, "dirt_pave.png");
	setTile(4, 5, -4, "dirt_pave.png");
	setTile(4, 6, -4, "dirt_pave.png");
	setTile(5, 6, -4, "dirt_pave.png");
	setTile(6, 6, -4, "dirt_pave.png");
	setTile(6, 5, -4, "dirt_pave.png");
	setTile(6, 4, -4, "dirt_pave.png");
	
	for(var x = 0; x < level.width; x++)
	{
		setTile(x, level.height - 1, 32, "dirt_gravel.png");
	}
}

var level = null;
fs.readFile(__dirname + "/resources/levels/main.json",
	function (err, data) {
		if (err) {
			level = Level.create(11, 11);
		} else {
			level = Level.load(data);
		}
		prepareLevel(level);
		console.log("Level ready!");
	});

function handler (req, res) {
	var uri = url.parse(req.url, true);
	var path = uri.pathname;
	if(path == "/") path = "/sites/index.html";
	
	fs.readFile(__dirname + "/../" + path,
	function (err, data) {
		if (err) {
			res.writeHead(500);
			return res.end('Error loading ' + path);
		}
		res.writeHead(200);
		res.end(data);
	});
}

var players = { }

io.on('connection', function (socket) {

	function handle(socket) {
		var player = {
			name: "",
			loggedIn: false,
			spawned: false,
			position: { x: 0.0, y: 0.0 },
			socket: socket
		};
		
		socket.emit("load-level", level);
		
		// Spawn all already-spawned players
		for(var key in players) {
			var p = players[key];
			if(!p.spawned) continue;
			socket.emit('spawn-player', {
				id: p.name,
				x: p.position.x,
				y: p.position.y,
			});
		}
		
		socket.on('login', function (data) {
			if(players[data.name] != undefined) {
				// Already logged in
				socket.emit('login-response', { success: false, reason: "already logged in" });
			} else {
				player.name = data.name;
				players[data.name] = player;
				global.emit('login', data);
			}
		});
		socket.on('request-spawn', function (data) {
			if(player.loggedIn) {
				player.spawned = true;
				io.emit('spawn-player', { id: player.name, x: 0.0, y:0.0 });
			} else {
				// TODO: Implement correct response
				console.log("try spawning non-logged-in player");
			}
		});
		socket.on('update-player', function (data) {
			player.position.x = data.x;
			player.position.y = data.y;
			
			io.emit('update-player', { id: player.name, x: data.x, y: data.y }); 
		});
		socket.on('disconnect', function (data) {
			console.log("disconnect.");
			console.log(data);
			
			if(player.spawned) {
				io.emit('despawn', { id: player.name });
			}
			
			delete players[player.name];
			player = undefined;
		});
	}
	handle(socket);
});

global.on('login-response', function (data) {
	var player = players[data.name]
	if(player === undefined) {
		console.log("Player " + data.name + " is not known to this server!");
	} else {
		player.loggedIn = data.success;
		if(data.success) {
			player.socket.emit('login-response', { success: true });
		} else {
			player.socket.emit('login-response', { success: false, reason: data.reason });
		}
	}
});