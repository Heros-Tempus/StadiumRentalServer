# StadiumRentalServer

Despite the name, this app does not use a client/server setup. I just named it that to keep the purpose of this app and its companion clear in my mind.

This app was made specifically to help facilitate a Pokemon Stadium tournament hosted by https://www.youtube.com/@DillonWithaBlankie. If anyone else wishes to use it, you will need to set up a MongoDB cluster because this app and its companion app utilize a MongoDB cluster as an intermediary to communicate. The connection string to the Mongo cluster needs to be stored in a text file named "ConnectionString" and be placed in the same directory as the app. The name of the connection string file must not include a file extension.

This app is meant to be used by the tournament announcer. The app reads the parties for every player in the database so that rental mons can be chosen. It also reads player inputs from the database, translates them into button commands for Pokemon Stadium, and deletes player inputs from the input database to ensure that next time player input is read that the new input was added deliberately.
