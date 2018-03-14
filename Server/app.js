//FINAL
var http = require('http');
var path = require('path');
var express = require('express');
var logger = require('morgan');
var bodyParser = require('body-parser');
var cookieParser = require('cookie-parser');
var passport = require('passport');
var session = require('express-session');
var io = require('socket.io')(process.envPort||4000);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://localhost:27017/";

var app = express();

app.set('views', path.resolve(__dirname, 'views'));
app.set('view engine', 'ejs');

app.use(logger("dev"));

app.use(bodyParser.urlencoded({extended:false}));
app.use(cookieParser());


app.use(session({
	secret:"secretSession", 
	resave:true,
	saveUninitialized:true

}));

app.use(passport.initialize());
app.use(passport.session());

passport.serializeUser(function(user, done){
	done(null,user);
});

passport.deserializeUser(function(user, done){
	done(null,user);
});

LocalStrategy = require('passport-local').Strategy;

passport.use(new LocalStrategy({
	usernameField:'',
	passworldField:''
	},
	function(username, password, done){
		MongoClient.connect(url, function(err, db){
			if(err)throw err;
			
			var dbObj = db.db("users");
			
			dbObj.collection("users").findOne({username:username}, function(err,results){
				if(results.password === password) {
					var user = results;
					done(null, user);
				}
				else {
					done(null, false, {message:'Bad Password'});
				
				};
			});
		});
		
	}));
	
	function ensureAuthenticated(req,res, next){
		if(req.isAuthenticated()){
			next();
		}
		else{
			res.redirect("/sign-in");
		}
	}
	
	app.get('/logout', function(req, res){
		req.logout();
		res.redirect("/sign-in");
	});

app.get("/", ensureAuthenticated, function(request,response){
	MongoClient.connect(url, function(err,db){
		if(err) throw err;
		var dbObj = db.db("users");
		
		
		dbObj.collection("questionData").find().toArray(function(err,results){
			console.log("Site Served");
			console.log({favorites:results});
			
			db.close();
			response.render("index",{favorites:results});
		});
	
	});
	
});

app.get("/new-entry", ensureAuthenticated, function(request,response){
	response.render("new-entry");	
});

app.get("/new-topic", ensureAuthenticated, function(request,response){
	response.render("new-topic");	
});

app.post("/new-topic", function(request,response){
	if(!request.body.title||!request.body.body) {
		response.status(400).send("Entries must have some text!");
		return;
	}
	//connected to our database and saved favorites
	MongoClient.connect(url,function(err, db){
		if(err)throw err;
		
		var dbObj = db.db("users");
		var topicOfChoice;
		
		dbObj.collection("users").findOne({username:username}, function(err, results){
			topicOfChoice = results.category;
		});
		
		dbObj.collection("topic").update(topicOfChoice, function(err,result){
			console.log("data saved");
			db.close();
			response.redirect("/");
			
		});
			
		
	});
	
	/*entries.push({
		title:request.body.title,
		body:request.body.body,
		published:new Date()
	});*/
	
	//response.redirect("/");

});


app.get("/sign-in", function(request,response){
	response.render("sign-in");	
});

app.post("/new-entry", function(request,response){
	if(!request.body.title||!request.body.answer1||!request.body.answer2||!request.body.answer3||!request.body.answer4) {
		response.status(400).send("Entries must have some text!");
		return;
	}
	
	console.log(request.body.isCorrect)
	
	var ans1 = false;
	var ans2 = false;
	var ans3 = false;
	var ans4 = false;
	
	if(request.body.isCorrect.includes('ans1'))
	{
		ans1 = true;
	}
	
	if(request.body.isCorrect.includes('ans2'))
	{
		ans2 = true;
	}
	if(request.body.isCorrect.includes('ans3'))
	{
		ans3 = true;
	}
	if(request.body.isCorrect.includes('ans4'))
	{
		ans4 = true;
	}
	
	//connected to our database and saved favorites
	MongoClient.connect(url,function(err, db){
		if(err)throw err;
		
		var dbObj = db.db("users");

		dbObj.collection("questionData").update({"_id": 500000},
			{
				$push: { questions: {questionText: request.body.title}},
				
			},
					 request.body, function(err,result){	
			
		});
		
		dbObj.collection("questionData").update({"_id": 500000, "questions.questionText": request.body.title},
			{
				$push: { "questions.$.answers": { $each: [ {answerText: request.body.answer1, isCorrect: ans1}, {answerText: request.body.answer2, isCorrect: ans2}, {answerText: request.body.answer3, isCorrect: ans3}, {answerText: request.body.answer4, isCorrect: ans4}] }
				
			}},
					 request.body, function(err,result){
						console.log("data saved");
						console.log(request.body.title)
						db.close();
						response.redirect("/");
			
		});
	});
	
	/*entries.push({
		title:request.body.title,
		body:request.body.body,
		published:new Date()
	});*/
	
	//response.redirect("/");

});

app.post("/delete", function(request,response){
	
	console.log(request.body.title);
	
	//connected to our database and saved favorites
	MongoClient.connect(url,function(err, db){
		if(err)throw err;
		
		var dbObj = db.db("users");

		
		
		dbObj.collection("questionData").update({"_id": 500000},
			{
				$pull: { questions: {questionText: request.body.title}},
			},
				request.body, function(err,result){
				
			console.log("data deleted");
			db.close();
			response.redirect("/");
			
		});
	});

});

app.post("/sign-up", function(request,response){
	
	
	console.log(request.body);
	MongoClient.connect(url,function(err, db){
		if(err)throw err;
		var dbObj = db.db("users");
		
		
		
		
		var user = {
			username: request.body.username,
			password: request.body.password,
			category: "Category not chosen"
			
		}
		
		dbObj.collection("users").insert(user,function(err, results){
			if(err)throw err;
			request.login(request.body, function(){
				response.redirect('/sign-in');
			});
		
		});
	
	});
	
});

app.post("/sign-in", passport.authenticate('local', {
	failureRedirect:'/sign-in'
}), function(request,response){
		response.redirect('/');
});

app.get('/profile', function(request,response){
		response.json(request.user);
	});

app.use(function(request,response){
	response.status(404).render("404");
});



http.createServer(app).listen(3000, function() {
	console.log("Favorites List server started on port 3000");
});

var players = [];

io.on('connection', function(socket){
	
	MongoClient.connect(url, function(err, db){
		if(err)throw err;
		
		var dbObj = db.db("users");
		
		var thisPlayerId = shortid.generate();
		
		players.push(thisPlayerId);
		
		console.log('client connected spawning player id: '+ thisPlayerId);
		
		
		socket.emit('OnConnected', {id:thisPlayerId});
		
		
		
		players.forEach(function(playerId){
			if(playerId == thisPlayerId) return;
			
			socket.emit('spawn player', {id:playerId});
			console.log("Adding a new player", playerId);
		});
		
		socket.on('move', function(data){
			
			data._id = 500000;
			console.log("Player position is: ", JSON.stringify(data));
			dbObj.collection("questionData").save(data, function(err, res){
			if(err)throw err;
			console.log("data saved to MongoDB");
		});
			
			
		});
		
		socket.on('disconnect', function(){
			console.log("Player Disconnected");
			players.splice(players.indexOf(thisPlayerId),1);
			socket.broadcast.emit('disconnected', {id:thisPlayerId});
		});
		
		
		
		socket.on('sendData', function(){
			
			
			dbObj.collection("questionData").find().toArray(function(err,results){
			console.log("sendData");
			
			socket.emit('getQuestions', {questionData:results});
			/*socket.emit('getAnswer1', {favorites:answer1});
			socket.emit('getAnswer2', {favorites:answer2});
			socket.emit('getAnswer3', {favorites:answer3});
			socket.emit('getAnswer4', {favorites:answer4});
			response.render("index",{favorites:results});*/
		});
		});
	});
});










