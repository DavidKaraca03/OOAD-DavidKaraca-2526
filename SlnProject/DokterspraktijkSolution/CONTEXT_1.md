# CONTEXT.md — Tijdelijk bestand, VERWIJDEREN voor inleveren!

## Huidige stand van het project

De solution is net aangemaakt met één project:
- `DokterspraktijkSolution.slnx` — solution
- `Lib/` — class library project (hernoemd van ClassLibrary)
- `CLAUDE.md` — agent instruction file

## Wat nog moet gebeuren

### Stap 1 — Projecten toevoegen
- WPF Application (.NET10) → naam `DokterApp`
- WPF Application (.NET10) → naam `PatientApp`

### Stap 2 — Referenties instellen
- DokterApp → verwijzing naar Lib
- PatientApp → verwijzing naar Lib

### Stap 3 — NuGet packages in Lib
- `System.Configuration.ConfigurationManager`
- `Microsoft.Data.SqlClient`

### Stap 4 — App.config in DokterApp en PatientApp
```xml
<configuration>
  <connectionStrings>
    <add name="connStr"
         connectionString="Data Source=(localdb)\mssqllocaldb;Initial Catalog=DokterspraktijkDB;Integrated Security=True" />
  </connectionStrings>
</configuration>
```

---

## Exact databaseschema (uit DokterspraktijkDB.sql)

### Tabel `Afspraak`
| Kolom | Type | Opmerking |
|---|---|---|
| id | int | PK, auto-increment |
| moment | datetime | datum én tijdstip in één veld |
| klacht | text | reden van de consultatie |
| patient_id | int | FK naar Patient |
| dokter_id | int | FK naar Dokter |

### Tabel `Dokter`
| Kolom | Type | Opmerking |
|---|---|---|
| id | int | PK, auto-increment |
| voornaam | nvarchar(50) | |
| achternaam | nvarchar(50) | |
| gsm | nchar(10) | nullable |
| email | nvarchar(100) | |
| paswoord | nvarchar(100) | SHA256 hash |
| profielfotodata | image | nullable, byte[] |
| rizivnummer | int | |
| isgeconventioneerd | tinyint | 0=nee, 1=ja |

### Tabel `Patient`
| Kolom | Type | Opmerking |
|---|---|---|
| id | int | PK, auto-increment |
| voornaam | nvarchar(50) | |
| achternaam | nvarchar(50) | |
| geslacht | int | 0=onbekend, 1=man, 2=vrouw |
| gsm | nchar(10) | nullable |
| email | nvarchar(100) | |
| paswoord | nvarchar(100) | SHA256 hash |
| geboortedatum | datetime | |
| profielfotodata | image | nullable, byte[] |
| notificaties | int | 0=Geen, 1=Mail, 2=Sms, 3=Beide |

---

## Klassen in Lib — correcte properties

### NotificatieType.cs (enum)
```csharp
public enum NotificatieType { Geen = 0, Mail = 1, Sms = 2, Beide = 3 }
```

### GeslachtType.cs (enum) — optioneel
```csharp
public enum GeslachtType { Onbekend = 0, Man = 1, Vrouw = 2 }
```

### Persoon.cs (abstracte superklasse)
Gemeenschappelijke velden voor Patient én Dokter:
- `Id` (int)
- `Voornaam` (string)
- `Achternaam` (string)
- `Gsm` (string, nullable)
- `Email` (string)
- `Paswoord` (string) — altijd SHA256 opslaan
- `Profielfotodata` (byte[], nullable)
- static methode `HashPaswoord(string paswoord)` → SHA256

### Patient.cs (erft van Persoon)
Extra properties:
- `Geslacht` (GeslachtType of int)
- `Geboortedatum` (DateTime)
- `Notificaties` (NotificatieType)

CRUD methodes:
- `static List<Patient> GetAll()`
- `static List<Patient> GetAll(string zoekterm)` — gefilterd op naam
- `static Patient GetById(int id)`
- `static Patient GetByEmail(string email)` — voor login
- `int InsertInDb()` — geeft nieuw id terug
- `void UpdateInDb()`
- `void DeleteFromDb()` — verwijdert ook gekoppelde afspraken!

### Dokter.cs (erft van Persoon)
Extra properties:
- `Rizivnummer` (int)
- `IsGeconventioneerd` (bool)

CRUD methodes:
- `static List<Dokter> GetAll()`
- `static Dokter GetById(int id)`
- `static Dokter GetByEmail(string email)` — voor login

### Afspraak.cs
Properties:
- `Id` (int)
- `Moment` (DateTime) — datum én tijd
- `Klacht` (string)
- `PatientId` (int)
- `DokterId` (int)

Aggregatie (read-only):
- `Patient Patient` → `Patient.GetById(PatientId)`
- `Dokter Dokter` → `Dokter.GetById(DokterId)`

CRUD methodes:
- `static List<Afspraak> GetByDokter(int dokterId, DateTime datum)`
- `static List<Afspraak> GetByPatient(int patientId)`
- `int InsertInDb()`
- `void DeleteFromDb()`

---

## Bestandsstructuur die aangemaakt moet worden

```
DokterspraktijkSolution/
├── Lib/
│   ├── NotificatieType.cs
│   ├── GeslachtType.cs
│   ├── Persoon.cs
│   ├── Patient.cs
│   ├── Dokter.cs
│   └── Afspraak.cs
├── DokterApp/
│   ├── App.config
│   ├── MainWindow.xaml       ← DockPanel: links knoppen, rechts Frame
│   └── Pages/
│       ├── StartPage.xaml
│       ├── LoginPage.xaml
│       ├── AfsprakenPage.xaml
│       ├── PatientenOverzichtPage.xaml
│       ├── PatientDetailsPage.xaml
│       ├── PatientNieuwPage.xaml
│       └── PatientWijzigenPage.xaml
└── PatientApp/
    ├── App.config
    ├── MainWindow.xaml       ← DockPanel: links knoppen, rechts Frame
    └── Pages/
        ├── StartPage.xaml
        ├── LoginPage.xaml
        ├── AfsprakenOverzichtPage.xaml
        ├── NieuweAfspraakPage.xaml
        ├── ProfielInfoPage.xaml
        └── ProfielWijzigenPage.xaml
```

---

## Kritieke regels (zie CLAUDE.md voor volledig overzicht)
- Nooit `var` — altijd expliciet type
- Nooit LINQ
- Nooit DataGrid/GridView/ListView
- Nooit databinding in XAML
- Nooit async/await
- Alle SQL enkel in Lib
- Foutmeldingen in TextBlock, nooit MessageBox
- Frame/Page voor navigatie

## VERWIJDER DIT BESTAND VOOR JE INLEVERT!
