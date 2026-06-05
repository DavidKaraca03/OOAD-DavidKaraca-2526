beginprompt 

Prompt:
Ik ben student informatica en moet een project maken voor mijn C#-les.
Het project is een helpdesk ticketsysteem. Het bestaat uit één WPF-app waar medewerkers tickets kunnen beheren, filteren, toevoegen en afsluiten. De app werkt via een class library waar alle CSV-logica inzit.
Het belangrijkste voor mij is dat ik echt alleen mag werken met wat ik in de les gezien heb. Dus geen var, geen LINQ, geen DataGrid, geen databinding, geen async/await — dat soort dingen. Alle code moet volgens de cursus geschreven zijn: string[] in plaats van var, foreach in plaats van LINQ, gewone properties zonder fancy syntax. Kijk elke keer goed na of de code die je schrijft daar aan voldoet, want als ik dat gebruik op mijn project krijg ik 0.
De structuur van het project:

CLHelpdesk — class library met alle klassen (Ticket, HardwareTicket, SoftwareTicket, Medewerker), de enum TicketPrioriteit en de klasse TicketRepository die alle CSV-logica bevat
WpfHelpdesk — WPF applicatie die enkel communiceert via de class library, geen enkele File.Read of File.Write in de code-behind

Belangrijke regels:

Alle CSV-code zit enkel in de class library, nooit in de code-behind
Geen aparte DataLayer of DataContext klasse
Klassen gebruiken public, private, properties met { get; set; }, constructors met this() en base() zoals in de cursus
abstract basisklasse Ticket met virtual GeefInfo() die overschreven wordt in HardwareTicket en SoftwareTicket
ToString() wordt gebruikt in de ListBox, GeefInfo() in het detailpaneel

De WPF-app heeft:

Filters: prioriteit (ComboBox), melder (TextBox), alleen open tickets (CheckBox)
ListBox met tickets (via ToString())
Detailpaneel rechts (via GeefInfo())
Formulier om nieuw ticket toe te voegen met validatie en foutmeldingen
Knop om ticket af te sluiten met bevestigingsdialoog

Ik had geen tijd om meerdere agents te gebruiken dus het ik het zogemaakt met dezelfde md file als de project.