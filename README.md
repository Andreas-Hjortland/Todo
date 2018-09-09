# ACOS TODO
This is a simple todo application created for a interview I had for ACOS. 

# Build
To build this project, you can use `dotnet build` from the command line or just
use the build menu option in visual studio.  Use `npm run build` to build the
frontend assets. Note that we will automatically build the frontend assets on
publish (`dotnet publish` or through Visual Studio).

# Debug
To debug this project, just open the solution in visual studio and run it with
normal debugging (`F5`). It can also be started from the command line with
`dotnet run`.  To debug the JS code you can use the command `npm start` in
the root of the `AcosTodo.Web` project. If you don't want to have a angular
development server, you can use `npm run ng -- build --watch` which will build
the frontend in watch mode so that you will always be up to date, and you can
use Kestrel directly.

# Database initialization
To initialize the database you need to run `dotnet ef database update` and you
should be good to go.