# Wärmefluss-Analyse

Berechnung des relativen durchschnittlichen Wärmeleistung anhand der Heizkörpertemperatur.

pRel = ((tCurrent - tRoom) / (tRef - tRoom)) POW coefficient

tCurrent: Heizkörper(Vorlauf)-Temperatur
tRoom: Zimmertemperatur (22°)
tRef: Referenztemperatur (55°)
coefficient: 1,3

Darstellung der aktuellen und durchschnittlichen relativen Wärmeleistung pro Tag

## Datenquelle
Hardware:
BTMETER BT-930A Temperaturrecorder (e.g. 
https://www.ebay.de/itm/166992968852)

Export der Aufzeichnung als Excel-Datei:

`| index | time | temp | humidity |`


