# MetraAPI
========

Simple .NET Chicagoland Metra API

### Usage:

Get All Metra Lines (no stations):
```c#
var lines = MetraAPI.GetLines();
```

Get All Metra Lines w/ Stations:
```c#
var lines = MetraAPI.GetLinesAndStations();
```

Get All Train Delays:
```c#
var delays = MetraAPI.GetAllTrainDelays();
```

Get Train Data From One Station To Another:
```c#
var lines = MetraAPI.GetLinesAndStations();

var upwLine = lines.Single(l => l.LookupName == "up-w");
var vpStation = upwLine.Stations.Single(s => s.Station == "VILLAPARK");
var ogilvyStation = upwLine.Stations.Single(s => s.Station == "OTC");

MetraAPI.GetTrainData(upwLine, vpStation, ogilvyStation);
```
or
```c#
MetraAPI.GetTrainData("up-w", "VILLAPARK", "OTC");
```

Get Train Numbers For Line:
```c#
var lines = MetraAPI.GetLines();

var upwLine = lines.Single(l => l.LookupName == "up-w");

var trainNumbers = MetraAPI.GetTrainNumbersForLine(upwLine);
```
or
```c#
MetraAPI.GetTrainNumbersForLine("UP West");
```

Get Train Schedule:
```c#
var lines = MetraAPI.GetLines();

var upwLine = lines.Single(l => l.LookupName == "up-w");

var trainNumbers = MetraAPI.GetTrainNumbersForLine(upwLine);

var trainSchedule = MetraAPI.GetTrainSchedule(upwLine, trainNumbers[0]);
```
or
```c#
var trainSchedule = MetraAPI.GetTrainSchedule("UP West", 50);
```
