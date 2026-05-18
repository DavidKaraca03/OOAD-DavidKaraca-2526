# CLAUDE.md — Dokterspraktijk Project
*Agent instruction file voor Claude Code / Cursor*

---

## Context & doelstelling

C# schoolproject aan een Belgische hogeschool: een app voor een dokterspraktijk.
Dit is **geen herhaling** van eerder geziene leerstof, maar een **vervolg** erop.
Er komen nieuwe technieken aan bod die zelfstandig bestudeerd worden (zie hieronder).

De agent mag **uitsluitend** technieken gebruiken die passen binnen de stijl en beperkingen van de cursus.
Gebruik nooit "modernere" of "handigere" alternatieven buiten de cursusscope, ook niet als ze cleaner lijken.
Bij twijfel: vraag bevestiging voor je een techniek gebruikt.

### Nieuwe technieken in dit project (nog niet volledig gekend)
- Werken met een **SQL databank** in C# via ADO.NET
- Aanmaken en gebruiken van een **class library**
- Lezen en schrijven van **afbeeldingen** in een databank als `byte[]`
- **Wachtwoord hashing** met SHA256
- Werken met **Frame / Page** navigatie in WPF
- Items **dynamisch toevoegen** aan een Grid/WrapPanel (zie SlnDemoItemsPanel)
- Nieuwe WPF controls: `PasswordBox`, `Calendar`

---

## Projectstructuur

```
DokterspraktijkSolution/
├── ClassLibrary/
│   ├── Persoon.cs               ← abstracte superklasse
│   ├── Patient.cs               ← erft van Persoon
│   ├── Dokter.cs                ← erft van Persoon
│   ├── Afspraak.cs
│   └── NotificatieType.cs       ← enum: Geen, Mail, Sms, Beide
├── DoctorApp/
│   ├── MainWindow.xaml          ← navigatiebalk links + Frame rechts
│   └── Pages/
│       ├── StartPage.xaml
│       ├── LoginPage.xaml
│       ├── AfsprakenPage.xaml
│       ├── PatientenOverzichtPage.xaml
│       ├── PatientDetailsPage.xaml
│       ├── PatientNieuwPage.xaml
│       └── PatientWijzigenPage.xaml
├── PatientApp/
│   ├── MainWindow.xaml          ← navigatiebalk links + Frame rechts
│   └── Pages/
│       ├── StartPage.xaml
│       ├── LoginPage.xaml
│       ├── AfsprakenOverzichtPage.xaml
│       ├── NieuweAfspraakPage.xaml
│       ├── ProfielInfoPage.xaml
│       └── ProfielWijzigenPage.xaml
└── documentatie.docx
```

---

## Database

- **DBMS:** SQL Server Express + SSMS
- **Connectiestring:** staat in `App.config` van beide WPF-projecten, ingelezen via `ConfigurationManager` in de ClassLibrary
- **Wachtwoorden:** SHA256-gehashed — altijd hashen vóór vergelijken of opslaan
- **Foto's:** opgeslagen als `byte[]` in de databank

---

## Taal & naamgeving

Schrijf **altijd in het Nederlands**: variabelenamen, methodes, commentaar — tenzij het .NET-sleutelwoorden zijn.

| Soort | Notatie | Voorbeeld |
|---|---|---|
| Klassen, properties, methodes | PascalCase | `Patient`, `HaalAlleAfspraken()` |
| Variabelen | camelCase | `geselecteerdPatient` |
| Privévelden | camelCase met `_` | `_connString` |
| Constanten | PascalCase | `MaxAantalPogingen` |
| Enum-waarden | PascalCase | `NotificatieType.Mail` |

---

## VERBODEN — geeft 0 op het project

Gebruik onderstaande constructies **nooit**, ook niet als AI ze voorstelt:

```
var                    → gebruik altijd een expliciet type
LINQ                   → geen .Where(), .Select(), .OrderBy(), .FirstOrDefault(), .Any(), .ToList()...
databinding            → geen {Binding} of DataContext in XAML, geen ObservableCollection
DataGrid / GridView / ListView  → verboden
async / await          → verboden
dynamic                → verboden
tuples                 → verboden  (geen (int x, string y) = ...)
out parameters         → verboden
ref parameters         → verboden
structs                → verboden
user controls          → verboden
type switches          → verboden  (geen switch(obj) { case Patient p: })
case guards            → verboden  (geen case int n when n > 0:)
invoke / Dispatcher    → verboden
expando objects        → verboden
Entity Framework       → verboden
break in lussen        → VERBODEN  (break is ALLEEN toegestaan in switch-case)
MessageBox voor fouten → verboden  (gebruik TextBlock in de UI)
int.Parse() / TryParse()→ gebruik Convert.ToInt32() i.p.v. Parse/TryParse
(int) casting          → gebruik Convert.ToInt32() voor string→int conversies
SQL buiten ClassLibrary→ alle queries uitsluitend in de class library
```

---

## Toegestane technieken

### Datatypes
```csharp
string  int  double  bool  char  decimal  byte  long  float
int?  bool?  double?   // nullable types zijn toegestaan
```

### Conversie — gebruik ALTIJD de Convert-klasse
```csharp
Convert.ToInt32(...)
Convert.ToDouble(...)
Convert.ToString(...)
Convert.ToBoolean(...)
```

### Selecties
```csharp
if (...) { } else if (...) { } else { }
switch (x) { case ...: ...; break; }   // break ALLEEN in switch-case
int max = a > b ? a : b;               // ternaire operator mag
```

### Iteraties
```csharp
while (...) { }
do { } while (...);
for (int i = 0; i < n; i++) { }
foreach (Type item in collectie) { }
// continue mag (gebruik spaarzaam)
// break is VERBODEN in while/do-while/for/foreach
```

### Methodes
```csharp
private ReturnType MethodeNaam(Type param1, Type param2) { }
// optionele parameters: private void Druk(string tekst, int grootte = 12) { }
// named parameters: Druk(tekst: "hallo", grootte: 14);
```

### Collecties
```csharp
// Array (vaste grootte):
string[] namen = new string[5];
int[] getallen = { 1, 2, 3 };
Array.Sort(arr);  Array.Reverse(arr);  Array.IndexOf(arr, waarde);
arr.Contains(waarde);  arr.Length;
string.Join(", ", arr);  "a b c".Split(' ');

// List (variabele grootte):
List<string> namen = new List<string>();
namen.Add("Jan");  namen.Remove("Jan");  namen.RemoveAt(0);
namen.Clear();  namen.Contains("Jan");  namen.Sort();  namen.Count;
```

### Klassen & overerving
```csharp
public abstract class Persoon
{
    public string Naam { get; set; }
    public string Email { get; set; }
    public Persoon() { }
    public Persoon(string naam, string email) { Naam = naam; Email = email; }
    public override string ToString() { return $"{Naam}"; }
}

public class Patient : Persoon
{
    public DateTime Geboortedatum { get; set; }
    public Patient() { }
    public Patient(string naam, string email) : base(naam, email) { }
}

// Subtype controleren:
if (obj is Patient) { Patient p = obj as Patient; }
```

### Enum
```csharp
// Definieer BUITEN de klasse, in de namespace:
public enum NotificatieType { Geen, Mail, Sms, Beide }

NotificatieType voorkeur = NotificatieType.Mail;
int getal = (int)voorkeur;
NotificatieType type = (NotificatieType)1;
Enum.TryParse("Mail", out NotificatieType type2);
string tekst = voorkeur.ToString();
```

### SQL in class library
```csharp
using Microsoft.Data.SqlClient;
using System.Configuration;

private static string _connString =
    ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

public static List<Patient> HaalAllePatientenOp()
{
    List<Patient> patienten = new List<Patient>();
    using (SqlConnection conn = new SqlConnection(_connString))
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("SELECT * FROM Patient", conn);
        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Patient p = new Patient();
            p.Id = Convert.ToInt32(reader["id"]);
            p.Naam = Convert.ToString(reader["naam"]);
            // nullable veld:
            p.Foto = reader["foto"] == DBNull.Value ? null : (byte[])reader["foto"];
            patienten.Add(p);
        }
    }
    return patienten;
}
```

### WPF: Frame / Page navigatie
```csharp
// In MainWindow.xaml: <Frame Name="frmMain" NavigationUIVisibility="Hidden" />

// Navigeren vanuit MainWindow:
frmMain.Content = new LoginPage();
frmMain.Content = new PatientDetailsPage(patientId);   // id meegeven via constructor

// Ingelogde gebruiker globaal bewaren:
Application.Current.Properties["ingelogdeDokter"] = dokter;

// Methode in MainWindow aanroepen vanuit een Page:
((DoctorApp.MainWindow)Application.Current.MainWindow).NavigeerNaarAfspraken();
```

### WPF: Dynamisch cards toevoegen (verplicht voor patiëntenoverzicht)
```csharp
// In XAML: <WrapPanel Name="pnlPatienten" />
// In code-behind:
pnlPatienten.Children.Clear();
foreach (Patient p in patienten)
{
    Border card = new Border();
    card.Width = 200;
    card.Margin = new Thickness(5);
    // voeg StackPanel, Image, Labels en Buttons toe via code
    pnlPatienten.Children.Add(card);
}
```

### Afbeeldingen lezen/schrijven
```csharp
// Schijf → byte[] (in applicatielaag):
byte[] fotoBytes = File.ReadAllBytes(bestandsPad);

// byte[] → BitmapImage voor WPF Image control (in applicatielaag):
BitmapImage bitmap = new BitmapImage();
using (MemoryStream ms = new MemoryStream(fotoBytes))
{
    bitmap.BeginInit();
    bitmap.CacheOption = BitmapCacheOption.OnLoad;
    bitmap.StreamSource = ms;
    bitmap.EndInit();
}
imgFoto.Source = bitmap;

// byte[] opslaan in DB (in class library):
cmd.Parameters.AddWithValue("@foto", fotoBytes ?? (object)DBNull.Value);

// byte[] ophalen uit DB (in class library):
byte[] foto = reader["foto"] == DBNull.Value ? null : (byte[])reader["foto"];
```

### SHA256 wachtwoord hashing
```csharp
// In Persoon.cs (ClassLibrary) — static methode:
public static string HashWachtwoord(string wachtwoord)
{
    using (System.Security.Cryptography.SHA256 sha256 =
           System.Security.Cryptography.SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(
            System.Text.Encoding.UTF8.GetBytes(wachtwoord));
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}

// Login-logica in WPF code-behind:
string ingevoerdHash = Persoon.HashWachtwoord(txtWachtwoord.Password);
Dokter dokter = Dokter.GetByEmail(txtEmail.Text);
if (dokter != null && dokter.Wachtwoord == ingevoerdHash) { /* succes */ }
```

### Exception handling (ALLEEN in applicatielaag)
```csharp
// ClassLibrary gooit:
if (!reader.Read())
    throw new KeyNotFoundException($"Patient met id {id} niet gevonden");

// WPF code-behind vangt op — NOOIT MessageBox voor fouten, altijd TextBlock:
try
{
    List<Patient> patienten = Patient.HaalAllePatientenOp();
}
catch (KeyNotFoundException ex)
{
    txtFoutmelding.Text = ex.Message;
    txtFoutmelding.Visibility = Visibility.Visible;
}
catch (SqlException ex)
{
    txtFoutmelding.Text = "Databasefout: " + ex.Message;
    txtFoutmelding.Visibility = Visibility.Visible;
}
catch (Exception ex)
{
    txtFoutmelding.Text = "Onverwachte fout: " + ex.Message;
    txtFoutmelding.Visibility = Visibility.Visible;
}
```

### Formuliervalidatie
```csharp
// Toon foutmeldingen in TextBlock, NOOIT via MessageBox:
private bool ValideerFormulier()
{
    txtFout.Text = "";
    if (string.IsNullOrWhiteSpace(txtVoornaam.Text))
    {
        txtFout.Text = "Voornaam is verplicht.";
        return false;
    }
    if (string.IsNullOrWhiteSpace(txtEmail.Text))
    {
        txtFout.Text = "E-mailadres is verplicht.";
        return false;
    }
    // ... meer controles
    return true;
}
```

### MessageBox — enkel voor bevestigingsdialogen
```csharp
// MAG: bevestiging vragen voor een destructieve actie
MessageBoxResult result = MessageBox.Show(
    "Weet je zeker dat je deze patiënt wil verwijderen?",
    "Bevestigen",
    MessageBoxButton.OKCancel,
    MessageBoxImage.Warning);
if (result == MessageBoxResult.OK) { patient.VerwijderUitDb(); }

// MAG NIET: foutmelding tonen via MessageBox → gebruik TextBlock
```

---

## Architectuurregels

### ClassLibrary
- Bevat de klassen `Persoon` (superklasse), `Patient`, `Dokter`, `Afspraak` en enum `NotificatieType`
- **Alle SQL-queries staan uitsluitend in de ClassLibrary** — nooit in DoctorApp of PatientApp
- CRUD-methodes (`OpslaanInDb()`, `VerwijderUitDb()`, `GetAll()`, `GetById()`, ...) zitten **in de klassen zelf** — geen aparte Repository of DataContext
- Klassen zijn **puur**: geen verwijzingen naar WPF-controls, geen `MessageBox`, geen `Console`
- ClassLibrary **gooit** excepties met `throw` — WPF-projecten **vangen** ze op met `catch`

### WPF-projecten
- Communiceren **uitsluitend** via methodes en properties van de ClassLibrary
- Bevatten **geen enkele SQL-query**
- Exception handling (`try-catch`) zit hier — nooit in de ClassLibrary
- Foutmeldingen worden getoond in een `TextBlock` in de UI

### Overerving
```
Persoon  (abstracte superklasse)
├── Patient
└── Dokter
```
Gemeenschappelijke properties (`Id`, `Voornaam`, `Familienaam`, `Email`, `Wachtwoord`, `Gsm`, `Foto`, `Notificaties`) zitten in `Persoon`.

---

## Commentaar — verplicht overal

```csharp
/// <summary>
/// Haalt alle patiënten op uit de databank.
/// </summary>
public static List<Patient> HaalAllePatientenOp()
{
    // lijst aanmaken om resultaten in op te slaan
    List<Patient> patienten = new List<Patient>();

    // verbinding openen en query uitvoeren
    using (SqlConnection conn = new SqlConnection(_connString))
    {
        conn.Open();
        SqlCommand cmd = new SqlCommand("SELECT * FROM Patient", conn);
        SqlDataReader reader = cmd.ExecuteReader();

        // resultaten inlezen en omzetten naar Patient-objecten
        while (reader.Read())
        {
            // ...
        }
    }

    // resultaat teruggeven
    return patienten;
}
```

Regels:
- Grote blokken (klassen, methodes) krijgen een `///`-commentaarblok bovenaan
- Verdeel de code in stukken van een paar regels, met één commentaarregel per stuk
- Scheid stukken met één blanco regel
- Schrijf commentaar **in het Nederlands**

---

## Git-workflow

- Minimum **5 zinvolle commits** gespreid in de tijd — geen alles-in-één-keer commit
- Commit per afgewerkte feature, suggestie:
  1. Projectstructuur + ClassLibrary skelet
  2. Login functionaliteit (beide apps)
  3. DoctorApp: afspraken + patiëntenoverzicht
  4. DoctorApp: CRUD patiënten
  5. PatientApp: afspraken + profiel

---

## Werkwijze voor de agent

- Werk **feature per feature** — nooit meerdere grote onderdelen tegelijk
- Vraag bevestiging voor je naar het volgende onderdeel gaat
- Controleer na elke wijziging of de code **bouwt** zonder fouten
- Gebruik **ask mode** om technieken uit te leggen, **edit mode** om te wijzigen
- Controleer altijd of gegenereerde code geen verboden technieken bevat
- Schrijf geen code in één grote dump — werk incrementeel
- Als iets onduidelijk is over de opgave: vraag om verduidelijking

---

## Testaccounts

**Dokters:**
- alexandre@dokterspraktijk.be / t9ZmRrAbSfCv
- khalid@dokterspraktijk.be / r6A5NjPDB8EJ
- marcel@dokterspraktijk.be / n9RZuxCtYVBY
- julie@dokterspraktijk.be / 3aA4YxjpFvAz
- shake@dokterspraktijk.be / 4wBR77EaHqHY

**Patiënten:**
- rogier.vanderlinde@odisee.be / klepketoe
- polina.kozlova@odisee.be / hushhush
- alle andere patiënten: wachtwoord = "test"

---

## Huidige status

Fase 0 afgerond — plan van aanpak en CLAUDE.md klaar.
Volgende stap: SQL dump installeren in SSMS en solution aanmaken in Visual Studio.
