# GoogleTimeline

## Installation
1. Clone
2. Setup database
   1. Add default connection string for an SqlServer database
   2. `dotnet ef database update` from the GoogleTimelineUI root directory
3. Run GoogleTimelineUI project

## Usage
Visit the "Timeline" tab to add, remove, or filter your timeline data.

Export location history (platshistorik) from your [google takeout](https://takeout.google.com/settings/takeout?pli=1).

Extract the archive and find your JSON data in "Takeout/.../Semantic History Location".
