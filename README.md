# Projektbeskrivelse: Lufthavns Informationssystem

## ✈️ Introduktion
I denne opgave skal I implementere et system til styring af flyafgange i en lufthavn. Systemet fungerer som en bro mellem flyselskaber og passagerer i realtid.

Systemet består af et centralt **Web API**, som flyselskaber benytter til at opdatere flyinformationer, samt en række **informationsskærme** (konsol-applikationer), der modtager og viser disse opdateringer via en message broker.

---

## 🏗️ Arkitektur
Systemet er opbygget som en distribueret arkitektur med følgende komponenter:

1.  **Airport Web API:** Et ASP.NET Core Web API (Controller-baseret). Fungerer som *Producer*.
2.  **Message Broker:** RabbitMQ til distribution af beskeder.
3.  **Flight Info Screen:** En eller flere konsol-applikationer. Fungerer som *Consumers*.



---

## 📋 Opgavekrav

### 1. Airport Web API
I skal udvikle et Web API, der fungerer som indgangspunkt for flyselskaberne.

* **POST-endpoint:** (`api/flights/`) Modtager information om en ny flyafgang.
* **UPDATE-endpoint:** Muliggør opdatering af eksisterende flyafgange (f.eks. ændring af `Status` eller `Gate`).
* **DELETE-endpoint:** Gør det muligt at fjerne en flyafgang fra systemet.
* **Messaging:** Når API'et modtager en opdatering, skal denne sendes som en besked til RabbitMQ.

#### Datamodel
En flyafgang skal som minimum indeholde følgende felter:

| Felt | Type | Eksempel |
| :--- | :--- | :--- |
| **FlightNumber** | `string` | "SK123" |
| **Destination** | `string` | "London LHR" |
| **DepartureTime** | `DateTime` | 2026-03-10T14:30:00 |
| **Gate** | `string` | "A12" |
| **Status** | `string` | "On Time", "Delayed", "Boarding", "Cancelled" |

### 2. RabbitMQ Konfiguration
I skal opsætte RabbitMQ til at håndtere kommunikationen:
* Vælg en passende Exchange-type (f.eks. `Fanout` til simpel distribution eller `Topic` til bonusopgaven).
* Sørg for, at API'et kan forbinde og sende JSON-serialiserede objekter.

### 3. Flight Info Screen (Console App)
I skal programmere en konsol-applikation, der simulerer en informationsskærm i lufthavnen.

* **Lytter:** Applikationen skal forbinde til RabbitMQ og lytte på en kø.
* **Visning:** Når en besked modtages, skal konsollen renses (`Console.Clear()`) og opdateres med de nyeste informationer.
* **Layout:** Layoutet skal være læsbart og professionelt (brug evt. tabeller i konsollen).

---

## 🌟 Bonusopgaver (Valgfrit)
Implementer forskellige "skærmtyper" ved hjælp af **RabbitMQ Topics**.

* **Segmentering:** Lav forskellige skærme til hhv. "Indenrigs" og "Udenrigs" terminaler.
* **Routing:** Modificer API'et så man kan sende information specifikt til én terminal.