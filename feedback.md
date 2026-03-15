# Feedback til Makkekk - Airport Information System

Rigtig godt arbejde med dit projekt! Du har fået de vigtigste dele på plads, og din informationsskærm ser professionel ud i konsollen.

## ASP.NET Core Web API
- **Endpoints:** Dine CRUD-operationer virker efter hensigten, og du bruger de rigtige HTTP-verber (POST, PUT, DELETE).
- **Statisk liste:** Det er helt fint til dette projekt, men i et rigtigt system ville vi bruge en database (f.eks. via Entity Framework Core).
- **Navngivning:** Jeg lagde mærke til metodenavne som `getFlights` og `UpdateFLight`. I C# bruger vi normalt **PascalCase** til metoder (`GetFlights` og `UpdateFlight`). Det gør din kode lettere at læse for andre C#-udviklere.

## RabbitMQ Integration
- **Flight Consumer:** Flot arbejde med at renske skærmen og bruge en tabel i konsollen. Det giver et rigtig godt overblik!
- **Besked-struktur:** Du sender hele fly-objektet som JSON, hvilket er præcis som opgaven foreskriver.

## Arkitektur & Dependency Injection (DI)
- **Dependency Injection:** Lige nu opretter din `FlightController` selv sin `MessageProducer` med `new`. En stor styrke ved ASP.NET Core er indbygget DI. Ved at lade systemet "injicere" din producer via constructoren, gør du din kode meget mere fleksibel og lettere at teste.
- **Interfaces:** Jeg har lavet en lille rettelse i din kode, hvor jeg har tilføjet et `IMessageProducer` interface. Det følger princippet om "Dependency Inversion", som gør koden mere modulær.

## Forslag til næste skridt
- Prøv at bruge en **enum** til fly-status i stedet for en `string`. Det fjerner risikoen for tastefejl (f.eks. "Boarding" vs "boarding").
- Overvej om din `MessageProducer` kunne genbruge sin RabbitMQ-forbindelse i stedet for at åbne en ny hver gang (ligesom singleton-mønsteret).

Super flot indsats! Du er godt på vej med distribuerede systemer.
