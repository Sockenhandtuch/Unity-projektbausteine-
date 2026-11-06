namespace Bausteine
{
    /// <summary>
    /// Alles, womit der Spieler per Tastendruck interagieren kann:
    /// Truhen, Schilder, Türen, NPCs, Felder, Rastbänke...
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Kurzer Hinweistext, z.B. "Öffnen", "Lesen", "Ernten".</summary>
        string Prompt { get; }

        /// <summary>Ist eine Interaktion gerade möglich? (Truhe schon offen → false)</summary>
        bool CanInteract(PlayerInteractor interactor);

        /// <summary>Führt die Interaktion aus.</summary>
        void Interact(PlayerInteractor interactor);
    }
}
