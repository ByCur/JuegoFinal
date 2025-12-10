using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;   // 👈 Importante para TextMeshPro

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del menú")]
    public GameObject mainPanel;
    public GameObject rulesPanel;
    public GameObject goalPanel;

    [Header("Nombre de la escena del juego")]
    public string gameSceneName = "Race"; // cámbialo por el nombre real de tu escena

    [Header("UI del Récord (TextMeshPro)")]
    public TMP_Text recordText;        // Texto donde se muestra el récord
    public string recordKey = "BestTime"; // Clave para guardar el récord en PlayerPrefs

    private void Start()
    {
        LoadRecord();   // Cargamos el récord al iniciar
        ShowMain();     // Mostramos el panel principal
    }

    // -------------------------------
    //          BOTONES
    // -------------------------------

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowRules()
    {
        mainPanel.SetActive(false);
        goalPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }

    public void ShowGoal()
    {
        mainPanel.SetActive(false);
        rulesPanel.SetActive(false);
        goalPanel.SetActive(true);
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        rulesPanel.SetActive(false);
        goalPanel.SetActive(false);
        LoadRecord();   // Por si ha cambiado el récord mientras jugabas
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // -------------------------------
    //          RÉCORD
    // -------------------------------

    // Cargar el récord guardado
    public void LoadRecord()
    {
        if (PlayerPrefs.HasKey(recordKey))
        {
            float bestTime = PlayerPrefs.GetFloat(recordKey);
            recordText.text = "RECORD: " + bestTime.ToString("F2") + " s";
        }
        else
        {
            recordText.text = "RECORD: --";
        }
    }

    // Botón Reset Record
    public void ResetRecord()
    {
        PlayerPrefs.DeleteKey(recordKey);
        recordText.text = "RECORD: --";
    }
}
