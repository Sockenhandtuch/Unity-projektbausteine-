using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Textbox mit Schreibmaschinen-Effekt (Zelda / Stardew).
    /// Erste Bestätigung: Text sofort komplett zeigen. Zweite: nächste Zeile.
    /// Während der Dialog offen ist, kann sich der Spieler nicht bewegen und die Zeit steht.
    /// Einmal pro Szene im HUD-Canvas anlegen.
    /// </summary>
    public class DialogueBox : MonoBehaviour
    {
        public static DialogueBox Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text textLabel;
        [SerializeField] private GameObject continueIndicator;
        [SerializeField] private float charactersPerSecond = 40f;

        private PlayerInputReader input;
        private string[] lines;
        private int lineIndex;
        private bool isTyping;
        private int openedFrame;
        private Coroutine typingRoutine;

        public bool IsOpen => panel.activeSelf;

        public event Action Opened;
        public event Action Closed;

        private void Awake()
        {
            Instance = this;
            panel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) input = player.GetComponent<PlayerInputReader>();
        }

        /// <summary>Öffnet den Dialog mit den angegebenen Zeilen.</summary>
        /// <param name="speaker">Name des Sprechers. null/leer = kein Namensfeld.</param>
        public void Show(string speaker, string[] newLines)
        {
            if (newLines == null || newLines.Length == 0) return;

            lines = newLines;
            lineIndex = 0;
            openedFrame = Time.frameCount; // Gleicher Tastendruck soll nicht sofort weiterblättern

            panel.SetActive(true);
            if (nameLabel != null)
            {
                nameLabel.gameObject.SetActive(!string.IsNullOrEmpty(speaker));
                nameLabel.text = speaker;
            }

            if (input != null) input.GameplayEnabled = false;
            if (GameClock.Instance != null) GameClock.Instance.IsPaused = true;

            ShowLine();
            Opened?.Invoke();
        }

        private void Update()
        {
            if (!IsOpen || input == null || Time.frameCount == openedFrame) return;
            if (!input.SubmitPressed) return;

            if (isTyping) FinishTyping();
            else Next();
        }

        private void Next()
        {
            lineIndex++;
            if (lineIndex >= lines.Length) Close();
            else ShowLine();
        }

        private void ShowLine()
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(TypeLine(lines[lineIndex]));
        }

        // Nutzt maxVisibleCharacters statt Text-Stückelung → Rich-Text-Tags bleiben intakt.
        private IEnumerator TypeLine(string line)
        {
            isTyping = true;
            if (continueIndicator != null) continueIndicator.SetActive(false);

            textLabel.text = line;
            textLabel.maxVisibleCharacters = 0;
            textLabel.ForceMeshUpdate();
            int total = textLabel.textInfo.characterCount;

            float visible = 0f;
            while (visible < total)
            {
                visible += charactersPerSecond * Time.unscaledDeltaTime;
                textLabel.maxVisibleCharacters = Mathf.FloorToInt(visible);
                yield return null;
            }

            FinishTyping();
        }

        private void FinishTyping()
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = null;
            textLabel.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
            if (continueIndicator != null) continueIndicator.SetActive(true);
        }

        public void Close()
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            panel.SetActive(false);
            isTyping = false;

            if (input != null) input.GameplayEnabled = true;
            if (GameClock.Instance != null) GameClock.Instance.IsPaused = false;
            Closed?.Invoke();
        }
    }
}
