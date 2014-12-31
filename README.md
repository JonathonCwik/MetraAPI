MetraAPI
========

Simple .NET Chicagoland Metra API

Usage:
```c#
var trainBatch = MetraAPI.GetNextTrainBatch("UP-W", "VILLAPARK", "OTC");

var lines = LineHandler.GetAllLines();

var stations = MetraAPI.GetStationsForLine("UP-W");
```
