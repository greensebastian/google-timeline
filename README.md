# GoogleTimeline

## Installation
1. Clone
2. Setup database
   1. Add default connection string for an SqlServer database
   2. `dotnet ef database update` from the GoogleTimelineUI root directory
3. Generate the required google-auth keys and ids, the microsoft docs have [a decent guide](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-5.0) for this
4. Run GoogleTimelineUI project

## Usage
Visit the "Timeline" tab to add, remove, or filter your timeline data.

Export location history (platshistorik) from your [google takeout](https://takeout.google.com/settings/takeout?pli=1).

Extract the archive and find your JSON data in "Takeout/.../Semantic History Location".
