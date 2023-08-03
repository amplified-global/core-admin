# Amplifi Admin Panel

Amplifi Admin Panel offers an interactive way to change content of metabase and tenants databases. Access to admin panel have users with `Amplifi admin` role, and it can be accessed by adding `/coreadmin` to the backend Url.

## Automatically creation of nuget package

Creation of nuget package is a part of github actions triggered when changes to the code are merged to master branch. To create new version of nuget package before commiting and pushing changes, the version of package has to be changed in `DotNetEd.CoreAdmin.csproj` file.

To change the version, just need to double click on project file, and look for `PackageVersion` property, and enter the new value of version.
